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
            worker.GoIdle();
            return;
        }

        worker.Animator.SetWorking(false);

        bool assigned = WorkerResourceSelector.TryAssignResourceAndSlot(worker);

        if (!assigned)
        {
            worker.GoIdle();
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
