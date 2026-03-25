using UnityEngine;

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

        if (worker == null)
            return;

        if (!worker.HasValidResourceAssignment())
        {
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
            return;
        }

        float distance = Vector2.Distance(worker.transform.position, worker.TargetSlot.Position);
        if (distance > worker.GetMaxWorkDistance())
        {
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
            return;
        }

        bool started = worker.TargetResource.TryStartWork(worker, OnFinished);

        if (!started)
        {
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
            return;
        }

        worker.Animator.SetWorking(true);
    }

    public void Update()
    {
        if (worker == null || finished)
            return;

        if (worker.TargetResource == null || worker.TargetSlot == null)
        {
            worker.Animator.SetWorking(false);
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
            return;
        }

        float distance = Vector2.Distance(worker.transform.position, worker.TargetSlot.Position);
        if (distance > worker.GetMaxWorkDistance())
        {
            worker.Animator.SetWorking(false);
            worker.ClearCurrentAssignment();
            worker.StartFindingResource();
        }
    }

    public void Exit()
    {
        worker.Animator.SetWorking(false);
    }

    private void OnFinished(int amount)
    {
        if (finished)
            return;

        finished = true;

        worker.Animator.SetWorking(false);
        worker.Inventory.SetCargo(amount);

        worker.ClearCurrentAssignment();

        worker.EnterCarryState();
    }
}
