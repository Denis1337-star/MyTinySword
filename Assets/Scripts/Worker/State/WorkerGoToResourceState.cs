using UnityEngine;

public class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker worker;
    private float repathTimer;
    private const float RepathInterval = 0.4f;

    public WorkerGoToResourceState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        repathTimer = RepathInterval;

        if (worker == null)
            return;

        if (!worker.HasValidResourceAssignment())
        {
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
            return;
        }

        worker.Movement.MoveTo(worker.TargetSlot.Position);
    }

    public void Update()
    {
        if (worker == null)
            return;

        if (!worker.HasValidResourceAssignment())
        {
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
            return;
        }

        float distance = Vector2.Distance(worker.transform.position, worker.TargetSlot.Position);

        if (distance <= worker.GetReachResourceDistance())
        {
            worker.Movement.Stop();
            worker.EnterWorkState();
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = RepathInterval;
            worker.Movement.MoveTo(worker.TargetSlot.Position);
        }

        if (!worker.Movement.HasTarget)
        {
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
        }
    }

    public void Exit()
    {
    }
}
