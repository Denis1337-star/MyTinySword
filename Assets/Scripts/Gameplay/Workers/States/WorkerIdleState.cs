using UnityEngine;

/// <summary>
/// Состояние ожидания worker'а
/// Worker идёт на idle-позицию у дома и периодически пытается продолжить работу
/// </summary>
public class WorkerIdleState : IWorkerState
{
    private readonly Worker worker;
    private float retryTimer;
    private const float RetryInterval = 0.35f;

    public WorkerIdleState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        retryTimer = RetryInterval;

        worker.Animator.SetWorking(false);
        worker.Animator.SetEquipment(EquipmentType.None);

        if (worker.Home != null)
            worker.Movement.MoveTo(worker.Home.GetIdlePosition(worker));
    }

    public void Update()
    {
        if (worker.Movement.HasTarget)   // Пока worker идёт к своей idle-позиции, не запускаем следующую логику
            return;

        retryTimer -= Time.deltaTime;
        if (retryTimer > 0f)
            return;

        retryTimer = RetryInterval;

        if (worker.PendingJob != WorkerJobType.None)   // Если есть отложенная работа — пробуем применить её
        {
            worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        if (worker.CurrentJob != WorkerJobType.None) // Если текущая работа уже назначена — пробуем снова искать ресурс
            worker.EnterFindResourceState();
    }

    public void Exit()
    {
    }
}
