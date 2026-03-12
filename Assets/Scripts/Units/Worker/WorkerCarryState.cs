using System.Collections;
using System.Collections.Generic;
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
        worker.Movement.MoveTo(worker.Home.DropPoint);
    }

    public void Update()
    {
        if (!worker.Movement.HasTarget)
        {
            if (worker.CurrentJobLogic != null && worker.CarriedAmount > 0)
                worker.CurrentJobLogic.GiveReward(worker.CarriedAmount);

            worker.CarriedAmount = 0;

            // После сдачи сначала применяем новую отложенную профессию
            if (worker.PendingJob != WorkerJobType.None)
            {
                worker.ApplyPendingJobIfAny();
                return;
            }

            worker.ChangeState(new WorkerFindResourceState(worker));
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
