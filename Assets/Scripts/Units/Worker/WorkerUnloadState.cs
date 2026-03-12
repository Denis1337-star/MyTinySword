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
        worker.Animator.PlayAction(WorkerAction.Idle);

        worker.CurrentJobLogic.GiveReward(worker.CarriedAmount);
        worker.CarriedAmount = 0;

        worker.ChangeState(new WorkerFindResourceState(worker));
    }

    public void Update() { }
    public void Exit() { }
}
