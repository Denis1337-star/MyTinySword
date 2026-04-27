using UnityEngine;
using Zenject;

/// <summary>
/// принимает решени€ о назначении и переключении работы worker
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
    /// Ќазначает новую работу рабочему
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
    /// ѕримен€ет отложенную работу, если она есть
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
    /// —разу примен€ет работу
    /// </summary>
    public void ApplyJobImmediately(WorkerJobType job)
    {
        if (_worker == null)
            return;

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