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

        bool started = worker.TargetResource.TryStartWork(worker, OnFinished);

        if (!started)
        {
            worker.Animator.SetWorking(false);

            if (worker.TargetResource != null)
                worker.TargetResource.CancelWork(worker);

            worker.TargetResource = null;
            worker.TargetSlot = null;

            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        worker.Animator.SetWorking(true);
    }

    private void OnFinished(int amount)
    {
        worker.Animator.SetWorking(false);

        worker.Inventory.SetCargo(amount);

        if (worker.TargetResource != null)
            worker.TargetResource.CancelWork(worker);

        worker.TargetResource = null;
        worker.TargetSlot = null;

        worker.ChangeState(new WorkerCarryState(worker));
    }

    public void Update() { }
    public void Exit() { }
}
