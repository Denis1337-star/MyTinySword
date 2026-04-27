/// <summary>
/// Состояние поиска ресурса для текущей работы worker
/// </summary>
public class WorkerFindResourceState : IWorkerState
{
    private readonly Worker _worker;

    public WorkerFindResourceState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        _worker.Animator?.SetWorking(false);

        bool assigned = WorkerResourceSelector.TryAssignResourceAndSlot(_worker);
        if (!assigned)
        {
            _worker.GoIdle();
            return;
        }

        _worker.Animator?.SetEquipment(GetToolForCurrentJob());
        _worker.Movement?.MoveTo(_worker.TargetSlot.Position);

        _worker.EnterGoToResourceState();
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }

    private EquipmentType GetToolForCurrentJob()
    {
        return _worker.CurrentJob switch
        {
            WorkerJobType.ChopWood => EquipmentType.Axe,
            WorkerJobType.MineGold => EquipmentType.Pickaxe,
            WorkerJobType.HuntMeat => EquipmentType.Knife,
            _ => EquipmentType.None
        };
    }
}
