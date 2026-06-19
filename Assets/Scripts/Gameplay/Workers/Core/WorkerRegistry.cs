using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех рабочих на сцене.
/// </summary>
public sealed class WorkerRegistry : MonoBehaviour
{
    private readonly List<Worker> _workers = new();
    private int _workerCounter;
    public IReadOnlyList<Worker> Workers => _workers;
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
    }

    public void Unregister(Worker worker)
    {
        if (worker == null)
            return;

        if (!_workers.Remove(worker))
            return;
    }

    private void OnDestroy()
    {
        _workers.Clear();
    }
}