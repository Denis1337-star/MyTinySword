using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Дом — владелец группы worker'ов
/// Отвечает за стартовый спавн, найм новых worker'ов
/// хранение локального списка своих рабочих,
/// выдачу idle-позиций и drop-позиций возле дома
/// </summary>
public class House : ValidatedMonoBehaviour
{
    [Header("Config")]
    [SerializeField] private HouseConfig config;

    [Header("Spawn & Drop")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform dropPoint;

    [Header("Idle Positions")]
    [SerializeField] private Transform idlePointsRoot;

    [Header("Workers")]
    [SerializeField] private Worker workerPrefab;

    [SerializeField] private float dropRadius = 0.6f;

    private readonly List<Transform> idlePoints = new();  // Все доступные idle-точки, собранные из idlePointsRoot
    private readonly Dictionary<Worker, Transform> occupiedIdlePoints = new();   // Какая idle-точка занята каким worker'ом
    private readonly List<Worker> workers = new();  // Локальный список worker'ов, принадлежащих этому дому

    private bool isHiringInProgress;

    public Vector2 DropPoint => dropPoint != null ? dropPoint.position : (Vector2)transform.position;

    public int MaxWorkers => config != null ? config.MaxWorkers : 0;
    public int CurrentWorkers => workers.Count;
    public IReadOnlyList<Worker> Workers => workers;

    public int CurrentWoodCost => config != null
        ? config.BaseWoodCost + CurrentWorkers * config.WoodIncreasePerWorker
        : 0;

    public int CurrentGoldCost => config != null
        ? config.BaseGoldCost + CurrentWorkers * config.GoldIncreasePerWorker
        : 0;

    public event Action OnWorkersChanged;
    public event Action<Worker> OnWorkerAdded;
    public event Action<Worker> OnWorkerRemoved;

    protected override void Awake()
    {
        CacheIdlePoints();
        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;
        // Проверяем обязательные ссылки
        valid &= ValidationUtility.NotEmptyCollection(this, config, nameof(config));
        valid &= ValidationUtility.NotEmptyCollection(this, spawnPoint, nameof(spawnPoint));
        valid &= ValidationUtility.NotEmptyCollection(this, workerPrefab, nameof(workerPrefab));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: HouseConfig is invalid", this);
            valid = false;
        }

        dropRadius = Mathf.Max(0f, dropRadius);
        return valid;
    }

    private void OnValidate()
    {
        dropRadius = Mathf.Max(0f, dropRadius);
    }

    private void Start()
    {
        if (!enabled)
            return;

        int startWorkers = config != null ? Mathf.Max(0, config.StartWorkers) : 0;

        // Спавним стартовых worker'ов, но не выходим за лимит
        for (int i = 0; i < startWorkers; i++)
        {
            if (CurrentWorkers >= MaxWorkers)
                break;

            SpawnWorker();
        }

        OnWorkersChanged?.Invoke();
    }
    /// <summary>
    /// Кэширует все дочерние idle-точки из idlePointsRoot
    /// </summary>
    private void CacheIdlePoints()
    {
        idlePoints.Clear();

        if (idlePointsRoot == null)
            return;

        foreach (Transform child in idlePointsRoot)
        {
            if (child != null)
                idlePoints.Add(child);
        }
    }
    /// Создаёт нового worker'а, привязывает его к дому
    /// и добавляет в локальный список дома
    /// </summary>
    private Worker SpawnWorker()
    {
        if (!enabled)
            return null;

        if (workerPrefab == null || spawnPoint == null)
        {
            Debug.LogError($"House {name}: workerPrefab or spawnPoint is missing.", this);
            return null;
        }

        Worker worker = Instantiate(workerPrefab, spawnPoint.position, Quaternion.identity);
        if (worker == null)
            return null;

        worker.SetHome(this);
        workers.Add(worker);

        Vector2 idlePos = GetIdlePosition(worker);
        worker.transform.position = idlePos;

        OnWorkerAdded?.Invoke(worker);
        OnWorkersChanged?.Invoke();

        return worker;
    }
    /// <summary>
    /// Можно ли сейчас нанять нового worker'а
    /// </summary>
    public bool CanHire()
    {
        return string.IsNullOrEmpty(GetHireBlockReason());
    }
    /// <summary>
    /// Возвращает причину, по которой найм сейчас невозможен
    /// Пустая строка означает, что найм разрешён
    /// </summary>
    public string GetHireBlockReason()
    {
        if (!enabled)
            return "Дом выключен из-за ошибки";

        if (config == null)
            return "Конфиг дома не назначен";

        if (isHiringInProgress)
            return "Найм уже выполняется";

        if (CurrentWorkers >= MaxWorkers)
            return "Достигнут лимит рабочих";

        if (ResourceStorage.Instance == null)
            return "Хранилище ресурсов не найдено";

        bool enoughWood = ResourceStorage.Instance.Wood >= CurrentWoodCost;
        bool enoughGold = ResourceStorage.Instance.Gold >= CurrentGoldCost;

        if (!enoughWood && !enoughGold)
            return "Не хватает дерева и золота";

        if (!enoughWood)
            return "Не хватает дерева";

        if (!enoughGold)
            return "Не хватает золота";

        return string.Empty;
    }
    /// <summary>
    /// Нанимает нового worker'а, если это возможно
    /// </summary>
    public void HireWorker()
    {
        if (isHiringInProgress)
            return;

        if (!CanHire())
            return;

        if (ResourceStorage.Instance == null)
            return;

        isHiringInProgress = true;

        try
        {
            bool spent = ResourceStorage.Instance.TrySpendResources(CurrentWoodCost, CurrentGoldCost);
            if (!spent)
                return;

            Worker worker = SpawnWorker();
            if (worker == null)
            {
                Debug.LogError($"House {name}: failed to spawn worker after spending resources.", this);
            }
        }
        finally
        {
            isHiringInProgress = false;
        }
    }
    /// <summary>
    /// Удаляет worker'а из локального списка дома
    /// и освобождает его idle-позицию
    /// </summary>
    public void RemoveWorker(Worker worker)
    {
        if (worker == null)
            return;

        if (!workers.Remove(worker))
            return;

        ReleaseIdlePosition(worker);
        OnWorkerRemoved?.Invoke(worker);
        OnWorkersChanged?.Invoke();
    }
    /// <summary>
    /// Возвращает idle-позицию для worker'а
    /// Если worker уже имеет закреплённую точку — возвращает её
    /// Иначе ищет первую свободную
    /// </summary>
    public Vector2 GetIdlePosition(Worker worker)
    {
        if (worker == null)
            return spawnPoint != null ? (Vector2)spawnPoint.position : (Vector2)transform.position;

        if (occupiedIdlePoints.TryGetValue(worker, out Transform existing) && existing != null)
            return existing.position;

        foreach (Transform point in idlePoints)
        {
            if (point == null)
                continue;

            if (!occupiedIdlePoints.ContainsValue(point))
            {
                occupiedIdlePoints[worker] = point;
                return point.position;
            }
        }

        return spawnPoint != null ? (Vector2)spawnPoint.position : (Vector2)transform.position;
    }
    /// <summary>
    /// Освобождает закреплённую idle-точку worker'а
    /// </summary>
    public void ReleaseIdlePosition(Worker worker)
    {
        if (worker == null)
            return;

        occupiedIdlePoints.Remove(worker);
    }
    /// <summary>
    /// Возвращает точку сдачи ресурсов для worker'а
    /// Worker'ы разводятся по окружности вокруг dropPoint,
    /// чтобы уменьшить скучивание возле дома.
    /// </summary>
    public Vector2 GetDropPosition(Worker worker)
    {
        Vector2 center = DropPoint;

        if (worker == null || workers.Count <= 1)
            return center;

        int index = workers.IndexOf(worker);
        if (index < 0)
            return center;

        float angleStep = 360f / Mathf.Max(1, workers.Count);
        float angle = angleStep * index * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dropRadius;
        return center + offset;
    }
}
