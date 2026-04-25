using UnityEngine;

/// <summary>
/// Состояние переноса добытого ресурса к дому
/// </summary>
public class WorkerCarryState : IWorkerState
{
    private readonly Worker worker;

    public WorkerCarryState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        worker.Animator?.SetWorking(false);
        worker.Animator?.SetEquipment(GetCarry());

        worker.Movement?.MoveTo(worker.Home.GetDropPosition(worker));
    }

    public void Update()
    {
        if (worker.Movement != null && worker.Movement.HasTarget)
            return;

        if (worker.HasCargo)
            worker.DeliverCargo();

        ContinueAfterDelivery();
    }

    public void Exit()
    {
    }

    private void ContinueAfterDelivery()
    {
        if (worker.PendingJob != WorkerJobType.None)
        {
            worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        if (worker.CurrentJob != WorkerJobType.None)
        {
            worker.StartFindingResource();
            return;
        }

        worker.GoIdle();
    }

    private EquipmentType GetCarry()
    {
        return worker.CurrentJob switch
        {
            WorkerJobType.ChopWood => EquipmentType.Wood,
            WorkerJobType.MineGold => EquipmentType.Gold,
            WorkerJobType.HuntMeat => EquipmentType.Meat,
            _ => EquipmentType.None
        };
    }
}
