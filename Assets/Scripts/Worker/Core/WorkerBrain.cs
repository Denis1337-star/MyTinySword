using UnityEngine;

public class WorkerBrain : MonoBehaviour
{
    private Worker worker;

    private void Awake()
    {
        worker = GetComponent<Worker>();
    }

    private void OnValidate()
    {
        if (worker == null)
            worker = GetComponent<Worker>();
    }

    public void AssignJob(WorkerJobType job)
    {
        if (worker == null || worker.Home == null)
            return;

        bool canSwitchNow = worker.CanSwitchJobImmediately();

        if (canSwitchNow || worker.CurrentJob == WorkerJobType.None)
        {
            ApplyJobImmediately(job);
            return;
        }

        worker.SetPendingJob(job);
    }

    public void ApplyPendingJobIfAny()
    {
        if (worker == null)
            return;

        if (worker.PendingJob == WorkerJobType.None)
            return;

        WorkerJobType nextJob = worker.PendingJob;
        worker.ClearPendingJob();
        ApplyJobImmediately(nextJob);
    }

    public void ApplyJobImmediately(WorkerJobType job)
    {
        if (worker == null)
            return;

        IWorkerJob logic = WorkerJobFactory.Create(job);

        worker.ResetTaskState();
        worker.SetCurrentJob(job, logic);

        if (job == WorkerJobType.None || logic == null)
        {
            worker.GoIdle();
            return;
        }

        worker.StartFindingResource();
    }
}
