using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех worker на сцене
/// уведомляет подписчиков об изменениях состава
/// </summary>
public class WorkerRegistry : MonoBehaviour
{
    public static WorkerRegistry Instance { get; private set; }

    public event Action<Worker> OnWorkerAdded;
    public event Action<Worker> OnWorkerRemoved;

    private readonly List<Worker> workers = new();
    private int workerCounter;

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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
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

        if (!workers.Remove(worker))
            return;

        OnWorkerRemoved?.Invoke(worker);
    }

    public bool Contains(Worker worker)
    {
        return worker != null && workers.Contains(worker);
    }
}
