using UnityEngine;

/// <summary>
/// Состояние движения к ресурсу
/// </summary>
public sealed class WorkerGoToResourceState : IWorkerState
{
    private const int MaxRepathAttempts = 1;

    private readonly Worker _worker;
    private readonly WorkerNavigationStuckTracker _stuckTracker = new();

    private float _reachDistanceSqr;
    private float _maxWorkDistanceSqr;
    private int _repathAttempts;

    public WorkerGoToResourceState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        _repathAttempts = 0;

        if (!_worker.HasValidResourceAssignment())
        {
            ResetToIdle();
            return;
        }

        float reachDistance = _worker.GetReachResourceDistance();
        float maxWorkDistance = _worker.GetMaxWorkDistance();

        _reachDistanceSqr = reachDistance * reachDistance;
        _maxWorkDistanceSqr = maxWorkDistance * maxWorkDistance;

        _stuckTracker.Reset(_worker.transform.position);

        if (!StartMovementToSlot())
            ResetToIdle();
    }

    public void Update()
    {
        if (!_worker.HasValidResourceAssignment())
        {
            ResetToIdle();
            return;
        }

        if (IsCloseEnoughToStartWork(_reachDistanceSqr))
        {
            StartWork();
            return;
        }

        if (_stuckTracker.Tick(_worker.Movement, _worker.transform.position, Time.deltaTime))
        {
            TryRecoverFromStuck();
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

    private void TryRecoverFromStuck()
    {
        if (_repathAttempts < MaxRepathAttempts)
        {
            _repathAttempts++;
            _stuckTracker.Reset(_worker.transform.position);

            if (StartMovementToSlot())
                return;
        }

        // Слот освобождается в ResetToIdle → овца снова свободна.
        ResetToIdle();
    }

    private bool StartMovementToSlot()
    {
        return _worker.Movement.MoveTo(_worker.TargetSlot.Position);
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
        _worker.ResetToIdle();
    }
}
