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

    [Header("Hire Cost")]
    public int hireWoodCost = 5;
    public int hireGoldCost = 2;

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

    #region Hire

    public bool CanHire()
    {
        if (CurrentWorkers >= maxWorkers)
            return false;

        if (!ResourceStorage.Instance.HasResources(hireWoodCost, hireGoldCost))
            return false;

        return true;
    }

    public void HireWorker()
    {
        if (!CanHire())
            return;

        ResourceStorage.Instance.SpendResources(hireWoodCost, hireGoldCost);

        Worker worker = Instantiate(workerPrefab, spawnPoint.position, Quaternion.identity);
        worker.SetHome(this);

        Vector2 idlePos = GetIdlePosition(worker);
        worker.transform.position = idlePos;

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
