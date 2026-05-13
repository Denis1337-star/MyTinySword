using UnityEngine;

/// <summary>
/// Состояние движения к ресурсу
/// </summary>
public sealed class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker _worker;

    private float _reachDistanceSqr;
    private float _maxWorkDistanceSqr;

    public WorkerGoToResourceState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        if (!_worker.HasValidResourceAssignmentForMove())
        {
            ResetToIdle();
            return;
        }

        float reachDistance = _worker.GetReachResourceDistance();
        float maxWorkDistance = _worker.GetMaxWorkDistance();

        _reachDistanceSqr = reachDistance * reachDistance;
        _maxWorkDistanceSqr = maxWorkDistance * maxWorkDistance;

        bool movementStarted = _worker.Movement.MoveTo(_worker.TargetSlot.Position);

        if (!movementStarted)
        {
            ResetToIdle();
        }
    }

    public void Update()
    {
        if (!_worker.HasValidResourceAssignmentForMove())
        {
            ResetToIdle();
            return;
        }

        if (IsCloseEnoughToStartWork(_reachDistanceSqr))
        {
            StartWork();
            return;
        }

        if (_worker.Movement.HasTarget)
            return;

        if (IsCloseEnoughToStartWork(_maxWorkDistanceSqr))
        {
            StartWork();
            return;
        }

        ResetToIdle();
    }

    public void Exit()
    {
    }

    private bool IsCloseEnoughToStartWork(float distanceSqr)
    {
        Vector2 workerPosition = _worker.transform.position;
        Vector2 targetPosition = _worker.TargetSlot.Position;

        float currentDistanceSqr = (workerPosition - targetPosition).sqrMagnitude;

        return currentDistanceSqr <= distanceSqr;
    }

    private void StartWork()
    {
        _worker.Movement.Stop();
        _worker.StateMachine.ChangeState(WorkerStateType.Work);
    }

    private void ResetToIdle()
    {
        _worker.ClearCurrentAssignment();
        _worker.StateMachine.ChangeState(WorkerStateType.Idle);
    }
}