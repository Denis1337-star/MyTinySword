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
        worker.TargetResource =
            worker.CurrentJobLogic.FindResource(worker.transform.position);

        if (worker.TargetResource == null)
        {
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }

        worker.TargetSlot = worker.TargetResource.GetFreeSlot(worker); // присвоение слота
        if (worker.TargetSlot == null)
        {
            worker.ChangeState(new WorkerIdleState(worker));
            return;
        }
        // Настраиваем анимацию и идём к ресурсу
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
