using UnityEngine;

/// <summary>
/// Состояние ожидания worker'а.
/// Рабочий стоит около дома и периодически проверяет,
/// есть ли текущая или отложенная работа.
/// </summary>
public class WorkerIdleState : IWorkerState
{
    private const float RetryInterval = 0.35f;

    private readonly Worker _worker;

    private float _retryTimer;

    public WorkerIdleState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        _retryTimer = RetryInterval;

        _worker.Animator?.SetWorking(false);
        _worker.Animator?.SetEquipment(EquipmentType.None);

        if (_worker.Home != null)
            _worker.Movement?.MoveTo(_worker.Home.GetIdlePosition(_worker));
    }

    public void Update()
    {
        _retryTimer -= Time.deltaTime;

        if (_retryTimer > 0f)
            return;

        _retryTimer = RetryInterval;

        if (_worker.PendingJob != WorkerJobType.None)
        {
            _worker.Brain.ApplyPendingJobIfAny();
            return;
        }

        if (_worker.CurrentJob != WorkerJobType.None)
            _worker.StartFindingResource();
    }

    public void Exit()
    {
    }
}
