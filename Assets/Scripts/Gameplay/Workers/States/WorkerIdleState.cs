using UnityEngine;

/// <summary>
/// —осто€ние ожидани€ worker
/// </summary>
public class WorkerIdleState : IWorkerState
{
    private readonly Worker worker;

    private float retryTimer;

    private const float RetryInterval = 0.35f; //через сколько снова ищет работу

    public WorkerIdleState(Worker worker)
    {
        this.worker = worker;
    }
    public void Enter()
    {
        retryTimer = RetryInterval;

        worker.Animator?.SetWorking(false);
        worker.Animator?.SetEquipment(EquipmentType.None);

        if (worker.Home != null)
            worker.Movement?.MoveTo(worker.Home.GetIdlePosition(worker));
    }
    public void Update()
    {
        retryTimer -= Time.deltaTime;

        if (retryTimer > 0f)
            return;

        retryTimer = RetryInterval;

        if (worker.PendingJob != WorkerJobType.None)
        {
            worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        if (worker.CurrentJob != WorkerJobType.None)
        {
            worker.StartFindingResource();
            return;
        }
    }
    public void Exit()
    {
    }
}
