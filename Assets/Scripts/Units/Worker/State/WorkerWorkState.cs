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
            worker.GoIdle();
            return;
        }

        worker.Animator.SetWorking(true);

        bool started = worker.TargetResource.TryStartWork(worker, OnFinished);

        if (!started)
        {
            worker.Animator.SetWorking(false);
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
        }
    }

    private void OnFinished(int amount)
    {
        worker.Animator.SetWorking(false);
        worker.Inventory.SetCargo(amount);
        worker.ClearCurrentAssignment();
        worker.ChangeState(new WorkerCarryState(worker));
    }

    public void Update() { }
    public void Exit() { }
}
