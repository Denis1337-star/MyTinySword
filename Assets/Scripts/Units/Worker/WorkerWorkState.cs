using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerWorkState : IWorkerState
{
    private readonly Worker worker;
    private bool isWorking;

    public WorkerWorkState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        worker.Animator.PlayAction(WorkerAction.Work);

        if (worker.TargetResource == null || worker.TargetSlot == null)
        {
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        isWorking = worker.TargetResource.TryStartWork(worker, OnFinished);

        if (!isWorking)
        {
            // если ресурс не доступен Idle
            worker.ChangeState(new WorkerIdleState(worker));
        }

       // worker.TargetResource.StartWork(OnFinished);
    }

    private void OnFinished(int amount)
    {
        worker.CarriedAmount = amount;

        worker.TargetResource.CancelWork(worker);

        worker.TargetResource = null;
        worker.TargetSlot = null;
        worker.ChangeState(new WorkerCarryState(worker));

    }

    public void Update() { }
    public void Exit() { }
}
