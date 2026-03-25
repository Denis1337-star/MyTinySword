using UnityEngine;

public class WorkerCarryState : IWorkerState
{
    private readonly Worker worker;

    public WorkerCarryState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        worker.Animator.SetWorking(false);
        worker.Animator.SetEquipment(GetCarry());

        if (worker.Home != null)
            worker.Movement.MoveTo(worker.Home.GetDropPosition(worker));
        else
            worker.GoIdle();
    }

    public void Update()
    {
        if (worker == null || worker.Home == null)
            return;

        if (!worker.Movement.HasTarget)
        {
            worker.DeliverCargo();

            if (worker.PendingJob != WorkerJobType.None)
            {
                worker.Brain.ApplyPendingJobIfAny();
                return;
            }

            worker.StartFindingResource();
        }
    }

    public void Exit() { }

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
