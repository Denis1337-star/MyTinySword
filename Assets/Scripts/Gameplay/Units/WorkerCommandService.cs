using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCommandService : MonoBehaviour
{
    public bool TryAssignJob(Worker worker, WorkerJobType job)
    {
        if (worker == null)
            return false;

        worker.AssignJob(job);
        return true;
    }
}
