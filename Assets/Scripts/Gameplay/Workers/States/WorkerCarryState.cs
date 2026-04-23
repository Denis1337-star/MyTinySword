using UnityEngine;

/// <summary>
/// Состояние переноса уже добытого ресурса к дому
/// Worker идёт в точку сдачи, выгружает груз в общую экономику
/// и затем либо применяет pending job, либо начинает новый рабочий цикл
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
        worker.Animator.SetWorking(false);
        worker.Animator.SetEquipment(GetCarry());

        // Если дом существует — идём в рассчитанную точку сдачи
        if (worker.Home != null)
            worker.Movement.MoveTo(worker.Home.GetDropPosition(worker));
        else
            worker.GoIdle();
    }

    public void Update()
    {
        // Без дома перенос невозможно завершить корректно
        if (worker.Home == null)
        {
            worker.GoIdle();
            return;
        }

        // Пока worker ещё движется к точке сдачи, ничего не делаем
        if (worker.Movement.HasTarget)
            return;

        // Если груз есть — сдаём его в общую экономику
        if (worker.HasCargo)
            worker.DeliverCargo();

        // Если игрок заранее назначил новую работу 
        // применяем её после завершения текущего цикла
        if (worker.PendingJob != WorkerJobType.None)
        {
            worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        // Иначе продолжаем текущую профессию и ищем новый ресурс
        worker.StartFindingResource();
    }

    public void Exit() { }

    /// <summary>
    /// Возвращает тип визуального груза в зависимости от текущей профессии worker'а
    /// </summary>
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
