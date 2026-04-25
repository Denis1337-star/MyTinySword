using System;
using UnityEngine;

/// <summary>
/// Состояние выполнения работы 
/// </summary>
public class WorkerWorkState : IWorkerState
{
    private readonly Worker worker;

    private bool finished;

    public WorkerWorkState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        finished = false;

        if (!worker.HasValidResourceAssignmentForWork())
        {
            RestartResourceSearch();
            return;
        }

        if (!IsWithinWorkDistance())
        {
            RestartResourceSearch();
            return;
        }

        bool started = worker.TargetResource.TryStartWork(worker, OnFinished);
        if (!started)
        {
            RestartResourceSearch();
            return;
        }

        worker.Animator?.SetWorking(true);
    }

    public void Update()
    {
        if (finished)
            return;

        if (!worker.HasValidResourceAssignmentForWork())
        {
            RestartResourceSearch();
            return;
        }

        if (!IsWithinWorkDistance())
            RestartResourceSearch();
    }

    public void Exit()
    {
        worker.Animator?.SetWorking(false);
    }

    private void OnFinished(int amount)
    {
        if (finished)
            return;

        finished = true;

        worker.Animator?.SetWorking(false);

        worker.Inventory.SetCargo(worker.CurrentJobLogic.RewardType, amount);
        worker.ClearCurrentAssignment();
        worker.EnterCarryState();
    }

    private bool IsWithinWorkDistance()
    {
        Vector2 currentPosition = worker.transform.position;
        Vector2 targetPosition = worker.TargetSlot.Position;
        float maxDistance = worker.GetMaxWorkDistance();

        return (targetPosition - currentPosition).sqrMagnitude <= maxDistance * maxDistance;
    }

    private void RestartResourceSearch()
    {
        worker.Animator?.SetWorking(false);
        worker.ClearCurrentAssignment();
        worker.StartFindingResource();
    }
}
