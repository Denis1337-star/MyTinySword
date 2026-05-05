using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// Дом — владелец группы worker
/// </summary>
public class House : ValidatedMonoBehaviour
{
    [Header("Config")]
    [SerializeField] private HouseConfig _config;

    [Header("Spawn & Drop")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _dropPoint;


    [Header("Idle Positions")]
    [SerializeField] private Transform _idlePointsRoot;

    [Header("Workers")]
    [SerializeField] private Worker _workerPrefab;

    [Header("Drop")]
    [SerializeField] private float _dropRadius = 0.6f;

    private readonly List<Transform> _idlePoints = new();
    private readonly Dictionary<Worker, Transform> _occupiedIdlePoints = new();
    private readonly List<Worker> _workers = new();

    private bool _isHiringInProgress;

    private WorkerFactory _workerFactory;
    private ResourceStorage _resourceStorage;

    public Vector2 DropPoint => _dropPoint.position;

    public int MaxWorkers => _config.MaxWorkers;
    public int CurrentWorkers => _workers.Count;
    public IReadOnlyList<Worker> Workers => _workers;

    public int CurrentWoodCost => _config.BaseWoodCost + CurrentWorkers * _config.WoodIncreasePerWorker;
    public int CurrentGoldCost => _config.BaseGoldCost + CurrentWorkers * _config.GoldIncreasePerWorker;

    public event Action OnWorkersChanged;
    public event Action<Worker> OnWorkerAdded;
    public event Action<Worker> OnWorkerRemoved;

    private readonly Subject<Unit> _workersChanged = new();
    private readonly Subject<Worker> _workerAdded = new();
    private readonly Subject<Worker> _workerRemoved = new();

    public IObservable<Unit> WorkersChanged => _workersChanged;
    public IObservable<Worker> WorkerAdded => _workerAdded;
    public IObservable<Worker> WorkerRemoved => _workerRemoved;

    [Inject]
    private void Construct(
        WorkerFactory workerFactory,
        ResourceStorage resourceStorage)
    {
        _workerFactory = workerFactory;
        _resourceStorage = resourceStorage;
    }

    protected override void Awake()
    {
        CacheIdlePoints();
        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _spawnPoint, nameof(_spawnPoint));
        valid &= ValidationUtility.IsAssigned(this, _workerPrefab, nameof(_workerPrefab));

        return valid;
    }

    private void OnValidate()
    {
        _dropRadius = Mathf.Max(0f, _dropRadius);
    }

    private void Start()
    {
        if (!enabled)
            return;

        int startWorkers = _config.StartWorkers;

        for (int i = 0; i < startWorkers; i++)
        {
            if (CurrentWorkers >= MaxWorkers)
                break;

            SpawnWorker();
        }

        OnWorkersChanged?.Invoke();
        _workersChanged.OnNext(Unit.Default);
    }

    private void CacheIdlePoints()
    {
        _idlePoints.Clear();

        if (_idlePointsRoot == null)
            return;

        foreach (Transform child in _idlePointsRoot)
        {
            if (child != null)
                _idlePoints.Add(child);
        }
    }

    private Worker SpawnWorker()
    {
        if (!enabled)
            return null;

        Worker worker = CreateWorker();
        if (worker == null)
            return null;

        _workers.Add(worker);
        worker.SetHome(this);

        Vector2 idlePosition = GetIdlePosition(worker);
        worker.transform.position = idlePosition;

        OnWorkerAdded?.Invoke(worker);
        OnWorkersChanged?.Invoke();

        return worker;
    }

    private Worker CreateWorker()
    {

        return _workerFactory.Create(
            _workerPrefab,
            _spawnPoint.position,
            Quaternion.identity);
    }

    public bool CanHire()
    {
        return string.IsNullOrEmpty(GetHireBlockReason());
    }

    public string GetHireBlockReason()
    {

        if (CurrentWorkers >= MaxWorkers)
            return "Достигнут лимит рабочих";

        bool enoughWood = _resourceStorage.Wood >= CurrentWoodCost;
        bool enoughGold = _resourceStorage.Gold >= CurrentGoldCost;

        if (!enoughWood && !enoughGold)
            return "Не хватает дерева и золота";

        if (!enoughWood)
            return "Не хватает дерева";

        if (!enoughGold)
            return "Не хватает золота";

        return string.Empty;
    }

    public void HireWorker()
    {
        if (_isHiringInProgress)
            return;

        if (!CanHire())
            return;

        _isHiringInProgress = true;

        try
        {
            bool spent = _resourceStorage.TrySpendResources(CurrentWoodCost, CurrentGoldCost);
            if (!spent)
                return;

            Worker worker = SpawnWorker();
            if (worker == null)
                Debug.LogError($"{name}: не удалось создать рабочего после списания ресурсов.", this);
        }
        finally
        {
            _isHiringInProgress = false;
        }
    }

    public void RemoveWorker(Worker worker)
    {
        if (worker == null)
            return;

        if (!_workers.Remove(worker))
            return;

        ReleaseIdlePosition(worker);

        OnWorkerRemoved?.Invoke(worker);
        OnWorkersChanged?.Invoke();
    }

    public Vector2 GetIdlePosition(Worker worker)
    {
        if (worker == null)
            return _spawnPoint.position;

        if (_occupiedIdlePoints.TryGetValue(worker, out Transform existing) && existing != null)
            return existing.position;

        foreach (Transform point in _idlePoints)
        {
            if (point == null)
                continue;

            if (_occupiedIdlePoints.ContainsValue(point))
                continue;

            _occupiedIdlePoints[worker] = point;
            return point.position;
        }

        return _spawnPoint.position;
    }

    public void ReleaseIdlePosition(Worker worker)
    {
        if (worker == null)
            return;

        _occupiedIdlePoints.Remove(worker);
    }
    public Vector2 GetDropPosition(Worker worker)
    {
        if (_dropPoint == null)
            return transform.position;

        if (worker == null)
            return _dropPoint.position;

        int index = _workers.IndexOf(worker);
        if (index < 0)
            index = 0;

        int workersPerRing = 8;
        int ring = index / workersPerRing + 1;
        int indexInRing = index % workersPerRing;

        float angle = indexInRing * Mathf.PI * 2f / workersPerRing;
        float radius = _dropRadius * ring;

        Vector2 offset = new(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius);

        return (Vector2)_dropPoint.position + offset;
    }
}
