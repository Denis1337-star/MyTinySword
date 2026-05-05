using UnityEngine;
using Zenject;

/// <summary>
/// Мозг worker'а.
/// Отвечает за назначение работы, отложенную смену работы
/// и применение job-логики через WorkerJobFactory.
/// </summary>
[RequireComponent(typeof(Worker))]
public sealed class WorkerBrain : MonoBehaviour
{
    private Worker _worker;
    private WorkerJobFactory _workerJobFactory;

    [Inject]
    private void Construct(WorkerJobFactory workerJobFactory)
    {
        _workerJobFactory = workerJobFactory;
    }

    private void Awake()
    {
        _worker = GetComponent<Worker>();
    }

    /// <summary>
    /// Назначает новую работу worker'у.
    /// Если worker занят, работа может попасть в PendingJob.
    /// </summary>
    public void AssignJob(WorkerJobType job)
    {
        if (_worker == null || _worker.Home == null)
            return;

        if (_worker.CurrentJob == job && _worker.PendingJob == WorkerJobType.None)
            return;

        if (_worker.PendingJob == job)
            return;

        bool canSwitchNow = _worker.CanSwitchJobImmediately();

        if (canSwitchNow ||
            _worker.CurrentJob == WorkerJobType.None ||
            job == WorkerJobType.None)
        {
            ApplyJobImmediately(job);
            return;
        }

        _worker.SetPendingJob(job);
    }

    /// <summary>
    /// Применяет отложенную работу, если она есть.
    /// Обычно вызывается после доставки ресурса домой.
    /// </summary>
    public void ApplyPendingJobIfAny()
    {
        if (_worker == null)
            return;

        if (_worker.PendingJob == WorkerJobType.None)
            return;

        WorkerJobType nextJob = _worker.PendingJob;

        _worker.ClearPendingJob();
        ApplyJobImmediately(nextJob);
    }

    /// <summary>
    /// Немедленно применяет новую работу.
    /// Сбрасывает текущее задание и запускает поиск ресурса.
    /// </summary>
    public void ApplyJobImmediately(WorkerJobType job)
    {
        if (_worker == null)
            return;

        if (_workerJobFactory == null)
        {
            Debug.LogError($"{name}: WorkerJobFactory не внедрён через Zenject.", this);
            return;
        }

        IWorkerJob logic = _workerJobFactory.Create(job);

        _worker.ResetTaskState();
        _worker.SetCurrentJob(job, logic);

        if (job == WorkerJobType.None || logic == null)
        {
            _worker.GoIdle();
            return;
        }

        _worker.StartFindingResource();
    }
}