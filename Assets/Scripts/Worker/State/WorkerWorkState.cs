using System;
using UnityEngine;

/// <summary>
/// —осто€ние выполнени€ работы на уже назначенном ресурсе
/// Worker запускает рабочую рутину ресурса, ждЄт завершени€,
/// получает награду в инвентарь и переходит в перенос ресурса
/// </summary>
public class WorkerWorkState : IWorkerState
{
    private readonly Worker worker;

    // «ащита от повторного завершени€ работы через callback
    private bool finished;

    public WorkerWorkState(Worker worker)
    {
        this.worker = worker ?? throw new ArgumentNullException(nameof(worker));
    }

    public void Enter()
    {
        finished = false;

        // ≈сли назначение на ресурс или слот уже невалидно Ч начинает поиск заново
        if (!worker.HasValidResourceAssignment())
        {
            RestartResourceSearch();
            return;
        }

        // ≈сли worker находитс€ слишком далеко от рабочей точки Ч не начинает работу
        if (!IsWithinWorkDistance())
        {
            RestartResourceSearch();
            return;
        }

        // ѕросит сам ресурс запустить свою рабочую рутину
        bool started = worker.TargetResource.TryStartWork(worker, OnFinished);
        if (!started)
        {
            RestartResourceSearch();
            return;
        }


        worker.Animator.SetWorking(true);
    }

    public void Update()
    {
        // ≈сли работа уже закончилась, состо€ние больше ничего не контролирует
        if (finished)
            return;

        // ¬о врем€ работы продолжает следить, что ресурс и слот всЄ ещЄ валидны
        if (!worker.HasValidResourceAssignment())
        {
            RestartResourceSearch();
            return;
        }

        // ≈сли worker по какой-то причине ушЄл слишком далеко от точки работы Ч
        // останавливает текущую фазу и отправл€ем его искать цель заново
        if (!IsWithinWorkDistance())
        {
            RestartResourceSearch();
        }
    }

    public void Exit()
    {
        // ѕри любом выходе из состо€ни€ гарантированно выключаем рабочую анимацию
        worker.Animator.SetWorking(false);
    }

    /// <summary>
    /// Callback, который вызываетс€ ресурсом после завершени€ работы
    /// amount Ч количество добытого ресурса
    /// </summary>
    private void OnFinished(int amount)
    {
        if (finished)
            return;

        finished = true;

        worker.Animator.SetWorking(false);

        // ≈сли ресурс вернул некорректное количество Ч просто перезапускает цикл поиска
        if (amount <= 0)
        {
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
            return;
        }

        //  ладЄт добытый ресурс в инвентарь worker'а
        worker.Inventory.SetCargo(amount);

        // ќсвобождаем текущий ресурс и слот
        worker.ClearCurrentAssignment();

        // ѕереходим в состо€ние переноса ресурса
        worker.EnterCarryState();
    }

    /// <summary>
    /// ѕровер€ет, находитс€ ли worker в допустимой дистанции дл€ выполнени€ работы
    /// </summary>
    private bool IsWithinWorkDistance()
    {
        Vector2 currentPosition = worker.transform.position;
        Vector2 targetPosition = worker.TargetSlot.Position;
        float maxDistance = worker.GetMaxWorkDistance();

        return (targetPosition - currentPosition).sqrMagnitude <= maxDistance * maxDistance;
    }

    /// <summary>
    /// выключаем анимацию, очищаем текущее назначение и запускаем поиск нового ресурса
    /// </summary>
    private void RestartResourceSearch()
    {
        worker.Animator.SetWorking(false);
        worker.ClearCurrentAssignment();
        worker.StartFindingResource();
    }
}
