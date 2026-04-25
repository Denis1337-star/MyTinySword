using UnityEngine;

/// <summary>
/// Состояние движения рабочего к слоту
/// </summary>
public class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker worker;

    private float repathTimer;

    private const float RepathInterval = 0.4f; //для повторного перестроение пути

    public WorkerGoToResourceState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        repathTimer = RepathInterval;

        if (!worker.HasValidResourceAssignmentForMove())
        {
            RestartResourceSearch();
            return;
        }

        worker.Movement?.MoveTo(worker.TargetSlot.Position);
    }

    public void Update()
    {
        if (!worker.HasValidResourceAssignmentForMove())
        {
            RestartResourceSearch();
            return;
        }

        float distanceSqr = ((Vector2)worker.transform.position - worker.TargetSlot.Position).sqrMagnitude;
        float reachDistance = worker.GetReachResourceDistance();

        if (distanceSqr <= reachDistance * reachDistance)
        {
            worker.Movement?.Stop();
            worker.EnterWorkState();
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = RepathInterval;
            worker.Movement?.MoveTo(worker.TargetSlot.Position);
        }

        if (worker.Movement != null && !worker.Movement.HasTarget)
            RestartResourceSearch();
    }

    public void Exit()
    {
    }

    private void RestartResourceSearch()
    {
        worker.ClearCurrentAssignment();
        worker.StartFindingResource();
    }
}
