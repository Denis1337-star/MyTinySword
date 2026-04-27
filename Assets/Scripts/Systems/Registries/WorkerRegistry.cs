using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех worker на сцене
/// </summary>
public class WorkerRegistry : MonoBehaviour
{
    private readonly List<Worker> _workers = new();
    private readonly Subject<Worker> _workerAdded = new();
    private readonly Subject<Worker> _workerRemoved = new();

    private int _workerCounter;

    public IReadOnlyList<Worker> Workers => _workers;

    public IObservable<Worker> WorkerAdded => _workerAdded;
    public IObservable<Worker> WorkerRemoved => _workerRemoved;

    public int Count => _workers.Count;

    public void Register(Worker worker)
    {
        if (worker == null)
            return;

        if (_workers.Contains(worker))
            return;

        _workerCounter++;
        worker.name = $"Worker {_workerCounter}";

        _workers.Add(worker);
        _workerAdded.OnNext(worker);
    }

    public void Unregister(Worker worker)
    {
        if (worker == null)
            return;

        if (!_workers.Remove(worker))
            return;

        _workerRemoved.OnNext(worker);
    }

    public bool Contains(Worker worker)
    {
        return worker != null && _workers.Contains(worker);
    }

    private void OnDestroy()
    {
        _workerAdded.OnCompleted();
        _workerRemoved.OnCompleted();

        _workerAdded.Dispose();
        _workerRemoved.Dispose();

        _workers.Clear();
    }
}
