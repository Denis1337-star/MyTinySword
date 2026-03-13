using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerUnloadState : IWorkerState
{
    private readonly Worker worker;

    public WorkerUnloadState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        worker.Animator.SetWorking(false);

        worker.DeliverCargo();

        // применяем отложенную работу через Brain
        if (worker.PendingJob != WorkerJobType.None)
        {
            worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        if (worker.CurrentJob != WorkerJobType.None)
            worker.ChangeState(new WorkerFindResourceState(worker));
        else
            worker.ChangeState(new WorkerIdleState(worker));
    }

    public void Update() { }

    public void Exit() { }
}
