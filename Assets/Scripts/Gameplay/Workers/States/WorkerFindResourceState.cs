using UnityEngine;
/// <summary>
/// Состояние поиска ресурса для текущей работы worker
/// </summary>
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

        worker.Animator?.SetWorking(false);
        worker.Animator?.SetEquipment(GetTool());

        bool assigned = WorkerResourceSelector.TryAssignResourceAndSlot(worker);
        if (!assigned)
        {
            worker.GoIdle();
            return;
        }

        if (!worker.HasValidResourceAssignmentForMove())
        {
            worker.ClearCurrentAssignment();
            worker.GoIdle();
            return;
        }

        worker.Movement?.MoveTo(worker.TargetSlot.Position);
        worker.EnterGoToResourceState();
    }
    public void Update()
    {
    }
    public void Exit()
    {
    }
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
