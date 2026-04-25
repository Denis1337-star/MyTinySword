using UnityEngine;

/// <summary>
/// —ервис команд дл€ worker
/// </summary>
public class WorkerCommandService : MonoBehaviour
{
    /// <summary>
    ///назначает worker новую работу
    /// </summary>
    public bool TryAssignJob(Worker worker, WorkerJobType job)
    {
        if (worker == null)
            return false;

        if (worker.CurrentJob == job && !worker.HasPendingJob)
            return false;

        if (worker.PendingJob == job)
            return false;

        worker.AssignJob(job);
        return true;
    }
}
