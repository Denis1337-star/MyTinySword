using UnityEngine;

/// <summary>
/// Состояние переноса добытого ресурса к дому
/// </summary>
public class WorkerCarryState : IWorkerState
{
    private readonly Worker _worker;

    public WorkerCarryState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        _worker.Animator?.SetWorking(false);
        _worker.Animator?.SetEquipment(GetCargoEquipment());

        if (_worker.Home == null)
        {
            _worker.Inventory?.Clear();
            _worker.GoIdle();
            return;
        }

        _worker.Movement?.MoveTo(_worker.Home.DropPoint);
    }

    public void Update()
    {
        if (_worker.Movement == null || _worker.Home == null)
            return;

        float distance = Vector2.Distance(
            _worker.transform.position,
            _worker.Home.DropPoint);

        if (distance > _worker.GetReachResourceDistance())
            return;

        _worker.Movement.Stop();
        _worker.DeliverCargo();

        if (_worker.PendingJob != WorkerJobType.None)
        {
            _worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        _worker.StartFindingResource();
    }

    public void Exit()
    {
    }

    private EquipmentType GetCargoEquipment()
    {
        if (_worker.CurrentJobLogic == null)
            return EquipmentType.None;

        return _worker.CurrentJobLogic.RewardType switch
        {
            ResourceType.Wood => EquipmentType.Wood,
            ResourceType.Gold => EquipmentType.Gold,
            ResourceType.Meat => EquipmentType.Meat,
            _ => EquipmentType.None
        };
    }
}
