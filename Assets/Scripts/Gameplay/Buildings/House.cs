using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [Header("Spawn & Drop")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform dropPoint;

    [Header("Idle Positions")]
    [SerializeField] private Transform idlePointsRoot;

    [Header("Workers")]
    [SerializeField] private int maxWorkers = 5;
    [SerializeField] private Worker workerPrefab;

    [Header("Hire Cost (Base)")]
    [SerializeField] private int baseWoodCost = 5;
    [SerializeField] private int baseGoldCost = 2;

    [Header("Hire Cost Increase")]
    [SerializeField] private int woodIncreasePerWorker = 2;
    [SerializeField] private int goldIncreasePerWorker = 1;

    [Header("Start Workers")]
    [SerializeField] private int startWorkers = 1;

    private readonly List<Transform> idlePoints = new();
    private readonly Dictionary<Worker, Transform> occupiedIdlePoints = new();
    private readonly List<Worker> workers = new();

    public Vector2 DropPoint => dropPoint != null ? dropPoint.position : transform.position;
    [SerializeField] private float dropRadius = 0.6f;

    public int MaxWorkers => maxWorkers;
    public int CurrentWorkers => workers.Count;
    public IReadOnlyList<Worker> Workers => workers;

    public int CurrentWoodCost => baseWoodCost + CurrentWorkers * woodIncreasePerWorker;
    public int CurrentGoldCost => baseGoldCost + CurrentWorkers * goldIncreasePerWorker;

    public event Action OnWorkersChanged;
    public event Action<Worker> OnWorkerAdded;
    public event Action<Worker> OnWorkerRemoved;

    private void Awake()
    {
        CacheIdlePoints();
    }

    private void Start()
    {
        for (int i = 0; i < startWorkers; i++)
        {
            SpawnWorker();
        }

        OnWorkersChanged?.Invoke();
    }

    private void CacheIdlePoints()
    {
        idlePoints.Clear();

        if (idlePointsRoot == null)
            return;

        foreach (Transform child in idlePointsRoot)
            idlePoints.Add(child);
    }

    private Worker SpawnWorker()
    {
        if (workerPrefab == null || spawnPoint == null)
        {
            Debug.LogError($"House {name}: workerPrefab или spawnPoint не назначен.", this);
            return null;
        }

        Worker worker = Instantiate(workerPrefab, spawnPoint.position, Quaternion.identity);

        worker.SetHome(this);
        workers.Add(worker);

        Vector2 idlePos = GetIdlePosition(worker);
        worker.transform.position = idlePos;

        OnWorkerAdded?.Invoke(worker);
        OnWorkersChanged?.Invoke();

        return worker;
    }

    public bool CanHire()
    {
        if (CurrentWorkers >= maxWorkers)
            return false;

        if (ResourceStorage.Instance == null)
            return false;

        return ResourceStorage.Instance.HasResources(CurrentWoodCost, CurrentGoldCost);
    }

    public void HireWorker()
    {
        if (!CanHire())
            return;

        ResourceStorage.Instance.SpendResources(CurrentWoodCost, CurrentGoldCost);
        SpawnWorker();
    }

    public void RemoveWorker(Worker worker)
    {
        if (worker == null)
            return;

        if (workers.Remove(worker))
        {
            ReleaseIdlePosition(worker);
            OnWorkerRemoved?.Invoke(worker);
            OnWorkersChanged?.Invoke();
        }
    }

    public Vector2 GetIdlePosition(Worker worker)
    {
        if (worker == null)
            return spawnPoint != null ? spawnPoint.position : transform.position;

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

        return spawnPoint != null ? spawnPoint.position : transform.position;
    }

    public void ReleaseIdlePosition(Worker worker)
    {
        if (worker == null)
            return;

        if (occupiedIdlePoints.ContainsKey(worker))
            occupiedIdlePoints.Remove(worker);
    }
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
