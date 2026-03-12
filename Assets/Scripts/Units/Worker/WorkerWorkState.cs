using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerWorkState : IWorkerState
{
    private readonly Worker worker;

    public WorkerWorkState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        if (worker.TargetResource == null || worker.TargetSlot == null)
        {
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        worker.Animator.SetWorking(true);

        bool started = worker.TargetResource.TryStartWork(worker, OnFinished);

        if (!started)
        {
            worker.TargetResource.CancelWork(worker);
            worker.TargetResource = null;
            worker.TargetSlot = null;
            worker.ChangeState(new WorkerFindResourceState(worker));
        }
    }

    private void OnFinished(int amount)
    {
        worker.Animator.SetWorking(false);
        worker.CarriedAmount = amount;

        if (worker.TargetResource != null)
            worker.TargetResource.CancelWork(worker);

        worker.TargetResource = null;
        worker.TargetSlot = null;

        worker.ChangeState(new WorkerCarryState(worker));
    }

    public void Update() { }
    public void Exit() { }
}
