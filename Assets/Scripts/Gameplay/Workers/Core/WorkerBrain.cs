using UnityEngine;
using Zenject;

/// <summary>
/// Отвечает за назначение работы
/// </summary>
[RequireComponent(typeof(Worker))]
public sealed class WorkerBrain : MonoBehaviour
{
    [SerializeField] private Worker _worker;

    private ResourceRegistry _resourceRegistry;

    private IWorkerJob _chopWoodJob;
    private IWorkerJob _mineGoldJob;
    private IWorkerJob _huntMeatJob;

    [Inject]
    private void Construct(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;

        _chopWoodJob = new ChopWoodJob(_resourceRegistry);
        _mineGoldJob = new MineGoldJob(_resourceRegistry);
        _huntMeatJob = new HuntMeatJob(_resourceRegistry);
    }

    /// <summary>
    /// Назначает новую работу worker
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
    /// Применяет отложенную работу
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
    /// Немедленно применяет новую работу
    /// </summary>
    public void ApplyJobImmediately(WorkerJobType job)
    {
        if (_worker == null)
            return;

        IWorkerJob logic = GetJobLogic(job);

        _worker.ResetTaskState();
        _worker.SetCurrentJob(job, logic);

        if (job == WorkerJobType.None || logic == null)
        {
            _worker.StateMachine.ChangeState(WorkerStateType.Idle);
            return;
        }

        _worker.StateMachine.ChangeState(WorkerStateType.FindResource);
    }

    private IWorkerJob GetJobLogic(WorkerJobType job)
    {
        return job switch
        {
            WorkerJobType.ChopWood => _chopWoodJob,
            WorkerJobType.MineGold => _mineGoldJob,
            WorkerJobType.HuntMeat => _huntMeatJob,
            _ => null
        };
    }
}