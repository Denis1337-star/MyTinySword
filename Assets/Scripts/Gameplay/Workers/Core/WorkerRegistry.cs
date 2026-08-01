using System.Collections.Generic;

/// <summary>
/// Глобальный реестр всех рабочих на сцене.
/// </summary>
public sealed class WorkerRegistry 
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
        worker.SetDisplayNumber(_workerCounter);
        // Hierarchy-имя только для отладки; в UI используется GameUiText.WorkerName.
        worker.name = $"Worker_{_workerCounter}";

        _workers.Add(worker);
    }

    public void Unregister(Worker worker)
    {
        if (worker == null)
            return;

        if (!_workers.Remove(worker))
            return;
    }
}