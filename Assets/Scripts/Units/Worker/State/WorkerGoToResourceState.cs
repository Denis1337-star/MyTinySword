using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker worker;
    private const float ReachDistance = 0.3f;

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

        float distance = Vector2.Distance(worker.transform.position, worker.TargetSlot.Position);

        if (distance <= ReachDistance || !worker.Movement.HasTarget)
        {
            worker.Movement.Stop();
            worker.ChangeState(new WorkerWorkState(worker));
        }
    }

    public void Exit()
    {
    }
}
