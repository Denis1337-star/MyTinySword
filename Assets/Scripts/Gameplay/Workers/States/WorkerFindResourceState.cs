using UnityEngine;

/// <summary>
/// —осто€ние поиска подход€щего ресурса дл€ текущей работы worker'а
/// ѕри успешном поиске подготавливает движение к ресурсу
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
        if (worker.CurrentJobLogic == null) // Ѕез job-логики worker не может пон€ть, какой ресурс искать
        {
            worker.GoIdle();
            return;
        }

        worker.Animator.SetWorking(false);
        worker.Animator.SetEquipment(GetTool());

        bool assigned = WorkerResourceSelector.TryAssignResourceAndSlot(worker);
        if (!assigned) // ≈сли ресурс или слот не найдены Ч уходим в безопасное idle-состо€ние
        {
            worker.GoIdle();
            return;
        }

        if (!worker.HasValidResourceAssignment())
        {
            worker.ClearCurrentAssignment();
            worker.GoIdle();
            return;
        }

        worker.Movement.MoveTo(worker.TargetSlot.Position);
        worker.EnterGoToResourceState();
    }

    public void Update() { }

    public void Exit() { }

    /// <summary>
    /// ¬озвращает инструмент, который должен отображатьс€ у worker'а
    /// при движении к ресурсу текущей профессии.
    /// </summary>
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
