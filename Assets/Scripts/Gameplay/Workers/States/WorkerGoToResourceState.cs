using UnityEngine;

/// <summary>
/// Состояние движения к ресурсу.
/// Worker идёт к зарезервированному рабочему слоту
/// и после прибытия переходит в состояние работы.
/// </summary>
public class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker _worker;

    public WorkerGoToResourceState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        if (!_worker.HasValidResourceAssignmentForMove())
        {
            _worker.ClearCurrentAssignment();
            _worker.GoIdle();
            return;
        }

        _worker.Movement?.MoveTo(_worker.TargetSlot.Position);
    }

    public void Update()
    {
        if (!_worker.HasValidResourceAssignmentForMove())
        {
            _worker.ClearCurrentAssignment();
            _worker.GoIdle();
            return;
        }

        if (_worker.Movement == null)
            return;

        float distance = Vector2.Distance(
            _worker.transform.position,
            _worker.TargetSlot.Position);

        if (distance > _worker.GetReachResourceDistance())
            return;

        _worker.Movement.Stop();
        _worker.EnterWorkState();
    }

    public void Exit()
    {
    }
}
