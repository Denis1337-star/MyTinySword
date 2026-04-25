using UnityEngine;

/// <summary>
/// принимает решени€ о назначении и переключении работы worker
/// </summary>
 [RequireComponent(typeof(Worker))]
public class WorkerBrain : MonoBehaviour
{
    private Worker worker;

    private void Awake()
    {
        worker = GetComponent<Worker>();
    }

    /// <summary>
    /// Ќазначает worker новую работу
    /// </summary>
    public void AssignJob(WorkerJobType job)
    {
        if (worker == null || worker.Home == null)
            return;

        if (worker.CurrentJob == job && worker.PendingJob == WorkerJobType.None)
            return;

        if (worker.PendingJob == job)
            return;

        bool canSwitchNow = worker.CanSwitchJobImmediately();

        if (canSwitchNow || worker.CurrentJob == WorkerJobType.None || job == WorkerJobType.None)
        {
            ApplyJobImmediately(job);
            return;
        }

        worker.SetPendingJob(job);
    }

    /// <summary>
    /// ѕримен€ет отложенную работу
    /// </summary>
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

    /// <summary>
    /// Ќемедленно примен€ет новую работу 
    /// </summary>
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
