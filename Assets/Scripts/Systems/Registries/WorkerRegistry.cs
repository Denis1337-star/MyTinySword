using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех worker'ов на сцене
/// Хранит актуальный список рабочих, позволяет регистрировать и удалять их,
/// а также уведомляет подписчиков об изменениях состава.
/// </summary>
public class WorkerRegistry : MonoBehaviour
{
    public static WorkerRegistry Instance { get; private set; }

    public event Action<Worker> OnWorkerAdded;
    public event Action<Worker> OnWorkerRemoved;

    private readonly List<Worker> workers = new();   // Внутренний список всех зарегистрированных worker'ов
    private int workerCounter;   // Счетчик для назначения читаемых имен новым worker'ам

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

    /// <summary>
    /// Регистрирует нового worker'а в глобальном реестре
    /// </summary>
    public void Register(Worker worker)
    {
        if (worker == null)
            return;

        if (workers.Contains(worker))  // Не допускаем повторной регистрации одного и того же объекта
            return;

        workerCounter++;
        worker.name = $"Worker {workerCounter}";

        workers.Add(worker);
        OnWorkerAdded?.Invoke(worker);
    }

    /// <summary>
    /// Удаляет worker'а из глобального реестра
    /// </summary>
    public void Unregister(Worker worker)
    {
        if (worker == null)
            return;

        if (!workers.Remove(worker))    // Если worker не был зарегистрирован, ничего не делает
            return;

        OnWorkerRemoved?.Invoke(worker);
    }

    /// <summary>
    /// Проверяет, содержится ли worker в реестре
    /// </summary>
    public bool Contains(Worker worker)
    {
        return worker != null && workers.Contains(worker);
    }
}
