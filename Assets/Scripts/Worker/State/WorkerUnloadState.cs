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

        if (worker.CurrentJobLogic != null && worker.CarriedAmount > 0)
            worker.CurrentJobLogic.GiveReward(worker.CarriedAmount);

        worker.CarriedAmount = 0;

        if (worker.CurrentJob != WorkerJobType.None)
            worker.ChangeState(new WorkerFindResourceState(worker));
        else
            worker.ChangeState(new WorkerIdleState(worker));
    }

    public void Update() { }
    public void Exit() { }
}
