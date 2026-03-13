using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerFindResourceState : IWorkerState
{
    private readonly Worker worker;

    public WorkerFindResourceState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        if (worker.CurrentJobLogic == null)
        {
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        worker.Animator.SetWorking(false);

        worker.TargetResource = worker.CurrentJobLogic.FindResource(worker.transform.position);

        if (worker.TargetResource == null)
        {
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        worker.TargetSlot = worker.TargetResource.GetFreeSlot(worker);

        if (worker.TargetSlot == null)
        {
            worker.TargetResource = null;
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        worker.Animator.SetEquipment(GetTool());
        worker.Movement.MoveTo(worker.TargetSlot.Position);
        worker.ChangeState(new WorkerGoToResourceState(worker));
    }

    public void Update() { }
    public void Exit() { }

    private EquipmentType GetTool()
    {
        return worker.CurrentJob switch
        {
            WorkerJobType.ChopWood => EquipmentType.Axe,
            WorkerJobType.MineGold => EquipmentType.Pickaxe,
            WorkerJobType.HuntMeat => EquipmentType.Knife,
            _ => EquipmentType.None
        };
    }
}
