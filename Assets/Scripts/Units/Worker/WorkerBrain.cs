using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBrain : MonoBehaviour
{
    private Worker worker;

    private void Awake()
    {
        worker = GetComponent<Worker>();
    }

    public void AssignJob(WorkerJobType job)
    {
        if (worker == null || worker.Home == null)
            return;

        bool canSwitchNow =
            worker.TargetResource == null &&
            worker.TargetSlot == null &&
            !worker.Inventory.HasCargo &&
            !worker.Movement.HasTarget;

        if (canSwitchNow || worker.CurrentJob == WorkerJobType.None)
        {
            ApplyJobImmediately(job);
            return;
        }

        worker.SetPendingJob(job);
    }

    public void ApplyPendingJobIfAny()
    {
        if (worker.PendingJob == WorkerJobType.None)
            return;

        WorkerJobType nextJob = worker.PendingJob;
        worker.ClearPendingJob();
        ApplyJobImmediately(nextJob);
    }

    public void ApplyJobImmediately(WorkerJobType job)
    {
        if (worker.TargetResource != null)
            worker.TargetResource.CancelWork(worker);

        worker.TargetResource = null;
        worker.TargetSlot = null;
        worker.Inventory.Clear();

        worker.SetCurrentJob(job, WorkerJobFactory.Create(job));
        worker.ChangeState(new WorkerFindResourceState(worker));
    }
}
