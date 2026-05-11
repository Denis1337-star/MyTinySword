using UnityEngine;

/// <summary>
/// Состояние движения к ресурсу
/// </summary>
public sealed class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker _worker;

    private float _reachDistanceSqr;

    public WorkerGoToResourceState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        if (!_worker.HasValidResourceAssignmentForMove())
        {
            _worker.ClearCurrentAssignment();
            _worker.StateMachine.ChangeState(WorkerStateType.Idle);
            return;
        }

        float reachDistance = _worker.GetReachResourceDistance();
        _reachDistanceSqr = reachDistance * reachDistance;

        _worker.Movement.MoveTo(_worker.TargetSlot.Position);
    }

    public void Update()
    {
        if (!_worker.HasValidResourceAssignmentForMove())
        {
            _worker.ClearCurrentAssignment();
            _worker.StateMachine.ChangeState(WorkerStateType.Idle);
            return;
        }

        Vector2 workerPosition = _worker.transform.position;
        Vector2 targetPosition = _worker.TargetSlot.Position;

        float sqrDistance = (workerPosition - targetPosition).sqrMagnitude;

        if (sqrDistance > _reachDistanceSqr)
            return;

        _worker.Movement.Stop();
        _worker.StateMachine.ChangeState(WorkerStateType.Work);
    }

    public void Exit()
    {
    }
}