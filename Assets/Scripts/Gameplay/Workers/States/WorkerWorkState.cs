using System;
using UnityEngine;

/// <summary>
/// Состояние выполнения работы 
/// </summary>
public class WorkerWorkState : IWorkerState
{
    private readonly Worker _worker;

    public WorkerWorkState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        if (!_worker.HasValidResourceAssignmentForWork())
        {
            _worker.ClearCurrentAssignment();
            _worker.GoIdle();
            return;
        }

        _worker.Animator?.SetWorking(true);

        bool started = _worker.TargetResource.TryStartWork(
            _worker,
            OnWorkFinished);

        if (!started)
        {
            _worker.Animator?.SetWorking(false);
            _worker.ClearCurrentAssignment();
            _worker.StartFindingResource();
        }
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _worker.Animator?.SetWorking(false);
    }

    private void OnWorkFinished(int amount)
    {
        if (amount <= 0)
        {
            _worker.ClearCurrentAssignment();
            _worker.StartFindingResource();
            return;
        }

        ResourceType resourceType = _worker.CurrentJobLogic != null
            ? _worker.CurrentJobLogic.RewardType
            : ResourceType.None;

        _worker.Inventory.SetCargo(resourceType, amount);
        _worker.ClearCurrentAssignment();
        _worker.EnterCarryState();
    }
}
