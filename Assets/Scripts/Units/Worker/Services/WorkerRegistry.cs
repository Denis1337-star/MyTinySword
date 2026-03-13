using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerRegistry : MonoBehaviour
{
    public static WorkerRegistry Instance { get; private set; }

    public event Action<Worker> OnWorkerAdded;
    public event Action<Worker> OnWorkerRemoved;

    private readonly List<Worker> workers = new();
    private int workerCounter = 0;

    public IReadOnlyList<Worker> Workers => workers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(Worker worker)
    {
        if (worker == null)
            return;

        if (workers.Contains(worker))
            return;

        workerCounter++;
        worker.name = $"Worker {workerCounter}";

        workers.Add(worker);
        OnWorkerAdded?.Invoke(worker);
    }

    public void Unregister(Worker worker)
    {
        if (worker == null)
            return;

        bool removed = workers.Remove(worker);
        if (!removed)
            return;

        OnWorkerRemoved?.Invoke(worker);
    }

    public bool Contains(Worker worker)
    {
        return worker != null && workers.Contains(worker);
    }
}
