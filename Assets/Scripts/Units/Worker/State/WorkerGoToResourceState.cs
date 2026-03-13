using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker worker;

    public WorkerGoToResourceState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
    }

    public void Update()
    {
        if (worker.TargetResource == null || worker.TargetSlot == null)
        {
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        if (Vector2.Distance(worker.transform.position, worker.TargetSlot.Position) <= 0.15f)
        {
            worker.Movement.Stop();
            worker.ChangeState(new WorkerWorkState(worker));
        }
    }

    public void Exit() { }
}
