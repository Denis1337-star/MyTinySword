using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerIdleState : IWorkerState
{
    private readonly Worker worker;
    private float retryTimer;
    private const float RetryInterval = 0.35f;

    public WorkerIdleState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        retryTimer = RetryInterval;

        worker.Animator.SetWorking(false);
        worker.Animator.SetEquipment(EquipmentType.None);

        if (worker.Home != null)
            worker.Movement.MoveTo(worker.Home.GetIdlePosition(worker));
    }

    public void Update()
    {
        if (worker == null || worker.Home == null)
            return;

        if (worker.Movement.HasTarget)
            return;

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
            worker.ChangeState(new WorkerFindResourceState(worker));
        }
    }

    public void Exit()
    {
    }
}
