using UnityEngine;

/// <summary>
/// Состояние переноски груза
/// </summary>
public sealed class WorkerCarryState : IWorkerState
{
    private readonly Worker _worker;

    private Vector2 _dropPosition;
    private float _reachDistanceSqr;

    public WorkerCarryState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        _worker.Animator.SetWorking(false);
        _worker.Animator.SetEquipment(GetCargoEquipment());

        if (_worker.Home == null)
        {
            _worker.Inventory.Clear();
            _worker.StateMachine.ChangeState(WorkerStateType.Idle);
            return;
        }

        _dropPosition = _worker.Home.GetDropPosition(_worker);

        float reachDistance = _worker.GetReachResourceDistance();
        _reachDistanceSqr = reachDistance * reachDistance;

        _worker.Movement.MoveTo(_dropPosition);
    }

    public void Update()
    {
        if (_worker.Home == null)
            return;

        Vector2 workerPosition = _worker.transform.position;
        float sqrDistance = (workerPosition - _dropPosition).sqrMagnitude;

        if (sqrDistance > _reachDistanceSqr)
            return;

        _worker.Movement.Stop();
        _worker.DeliverCargo();

        if (_worker.PendingJob != WorkerJobType.None)
        {
            _worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        WorkerStateType nextState = _worker.CurrentJob != WorkerJobType.None
            ? WorkerStateType.FindResource
            : WorkerStateType.Idle;

        _worker.StateMachine.ChangeState(nextState);
    }

    public void Exit()
    {
    }

    private EquipmentType GetCargoEquipment()
    {
        return _worker.Inventory.CarriedResourceType switch
        {
            ResourceType.Wood => EquipmentType.Wood,
            ResourceType.Gold => EquipmentType.Gold,
            ResourceType.Meat => EquipmentType.Meat,
            _ => EquipmentType.None
        };
    }
}