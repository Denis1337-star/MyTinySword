using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Дом  владелец группы рабочих
/// </summary>
public sealed class House : ValidatedMonoBehaviour
{
    private const int WorkersPerDropRing = 8;
    private const float FullCircleRadians = Mathf.PI * 2f;

    [SerializeField] private HouseConfig _config;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _dropPoint;
    [SerializeField] private Transform _idlePointsRoot;
    [SerializeField] private Worker _workerPrefab;
    [SerializeField] private float _dropRadius = 0.6f;

    private readonly List<Transform> _idlePoints = new();
    private readonly List<Worker> _workers = new();

    private WorkerFactory _workerFactory;
    private ResourceStorage _resourceStorage;

    private bool _isHiringInProgress;

    public Vector2 DropPoint => _dropPoint.position;

    public int MaxWorkers => _config.MaxWorkers;
    public int CurrentWorkers => _workers.Count;
    public IReadOnlyList<Worker> Workers => _workers;

    public int CurrentWoodCost => _config.BaseWoodCost + CurrentWorkers * _config.WoodIncreasePerWorker;
    public int CurrentGoldCost => _config.BaseGoldCost + CurrentWorkers * _config.GoldIncreasePerWorker;

    public event Action OnWorkersChanged;

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
        valid &= ValidationUtility.IsAssigned(this, _dropPoint, nameof(_dropPoint));
        valid &= ValidationUtility.IsAssigned(this, _workerPrefab, nameof(_workerPrefab));

        if (_config != null && !_config.IsValid())
        {
            Debug.LogError($"{name}: HouseConfig настроен некорректно.", this);
            valid = false;
        }

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
    }

    private void CacheIdlePoints()
    {
        _idlePoints.Clear();

        if (_idlePointsRoot == null)
            return;

        for (int i = 0; i < _idlePointsRoot.childCount; i++)
        {
            Transform child = _idlePointsRoot.GetChild(i);

            if (child != null)
                _idlePoints.Add(child);
        }
    }

    private Worker SpawnWorker()
    {
        Worker worker = CreateWorker();

        if (worker == null)
            return null;

        _workers.Add(worker);
        worker.SetHome(this);

        Vector2 idlePosition = GetIdlePosition(worker);
        worker.transform.position = idlePosition;

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

        if (!_resourceStorage.HasResources(CurrentWoodCost, CurrentGoldCost, 0))
            return "Не хватает ресурсов";

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
            bool spent = _resourceStorage.TrySpendResources(
                CurrentWoodCost,
                CurrentGoldCost,
                0);

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

        OnWorkersChanged?.Invoke();
    }

    public Vector2 GetIdlePosition(Worker worker)
    {
        int index = GetWorkerIndex(worker);

        if (index < 0 || index >= _idlePoints.Count)
            return _spawnPoint.position;

        Transform idlePoint = _idlePoints[index];

        return idlePoint != null
            ? idlePoint.position
            : _spawnPoint.position;
    }

    public Vector2 GetDropPosition(Worker worker)
    {
        int index = GetWorkerIndex(worker);

        if (index < 0)
            index = 0;

        int ring = index / WorkersPerDropRing + 1;
        int indexInRing = index % WorkersPerDropRing;

        float angle = indexInRing * FullCircleRadians / WorkersPerDropRing;
        float radius = _dropRadius * ring;

        Vector2 offset = new(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius);

        return (Vector2)_dropPoint.position + offset;
    }

    private int GetWorkerIndex(Worker worker)
    {
        if (worker == null)
            return -1;

        return _workers.IndexOf(worker);
    }
}