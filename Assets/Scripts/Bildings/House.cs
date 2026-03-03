using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private Transform dropPoint;
    public Vector2 DropPoint => dropPoint.position;

    [SerializeField] private int maxWorkers = 5;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Worker workerPrefab;

    [SerializeField] public int hireWoodCost = 5;
    [SerializeField] public int hireGoldCost = 2;

    public int MaxWorkers => maxWorkers;
    public int CurrentWorkers => WorkerRegistry.Instance.Workers.Count;

    public event Action OnWorkersChanged;

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

        Instantiate(workerPrefab, spawnPoint.position, Quaternion.identity);

        OnWorkersChanged?.Invoke();
    }
}
