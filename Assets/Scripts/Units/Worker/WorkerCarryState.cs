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
        worker.Animator.SetEquipment(GetCarry());
        worker.Movement.MoveTo(worker.Home.DropPoint);
    }

    public void Update()
    {
        if (!worker.Movement.HasTarget)
        {
            worker.CurrentJobLogic.GiveReward(worker.CarriedAmount); // кладём ресурс
            worker.CarriedAmount = 0;
            worker.ChangeState(new WorkerFindResourceState(worker)); // снова ищем ресурс
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
