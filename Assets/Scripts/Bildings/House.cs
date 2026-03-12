using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [Header("Spawn & Drop")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform dropPoint;
    public Vector2 DropPoint => dropPoint.position;

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

    public int MaxWorkers => maxWorkers;
    public int CurrentWorkers => WorkerRegistry.Instance.Workers.Count;

    public event Action OnWorkersChanged;

    private void Awake()
    {
        foreach (Transform child in idlePointsRoot)
            idlePoints.Add(child);
    }
    private void Start()
    {
        for (int i = 0; i < startWorkers; i++)
        {
            SpawnWorker();
        }
    }
    private Worker SpawnWorker()
    {
        Worker worker = Instantiate(workerPrefab, spawnPoint.position, Quaternion.identity);

        worker.SetHome(this);

        var selectable = worker.GetComponent<UnitSelectable>();
        if (selectable == null) selectable = worker.gameObject.AddComponent<UnitSelectable>();

        Vector2 idlePos = GetIdlePosition(worker);
        worker.transform.position = idlePos;

        return worker;
    }

    #region Hire
    public int CurrentWoodCost =>
    baseWoodCost + CurrentWorkers * woodIncreasePerWorker;

    public int CurrentGoldCost =>
        baseGoldCost + CurrentWorkers * goldIncreasePerWorker;

    public bool CanHire()
    {
        if (CurrentWorkers >= maxWorkers)
            return false;

        return ResourceStorage.Instance.HasResources(
            CurrentWoodCost,
            CurrentGoldCost
        );
    }

    public void HireWorker()
    {
        if (!CanHire())
            return;

        ResourceStorage.Instance.SpendResources(
            CurrentWoodCost,
            CurrentGoldCost
        );

        SpawnWorker();

        OnWorkersChanged?.Invoke();
    }

    #endregion

    #region Idle Positions

    public Vector2 GetIdlePosition(Worker worker)
    {
        if (occupiedIdlePoints.TryGetValue(worker, out Transform existing))
            return existing.position;

        foreach (var point in idlePoints)
        {
            if (!occupiedIdlePoints.ContainsValue(point))
            {
                occupiedIdlePoints[worker] = point;
                return point.position;
            }
        }

        // fallback Ч если точек не хватило
        return spawnPoint.position;
    }

    public void ReleaseIdlePosition(Worker worker)
    {
        if (occupiedIdlePoints.ContainsKey(worker))
            occupiedIdlePoints.Remove(worker);
    }

    #endregion
}
