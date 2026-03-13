using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorkerResourceSelector
{
    public static ResourceNodeBase FindBestResource(Worker worker)
    {
        if (worker == null || worker.CurrentJobLogic == null)
            return null;

        return worker.CurrentJobLogic.FindResource(worker.transform.position);
    }

    public static WorkSlot ReserveBestSlot(Worker worker, ResourceNodeBase resource)
    {
        if (worker == null || resource == null)
            return null;

        return resource.TryReserveSlot(worker);
    }

    public static bool TryAssignResourceAndSlot(Worker worker)
    {
        if (worker == null || worker.CurrentJobLogic == null)
            return false;

        ResourceNodeBase resource = FindBestResource(worker);
        if (resource == null)
            return false;

        WorkSlot slot = ReserveBestSlot(worker, resource);
        if (slot == null)
            return false;

        worker.TargetResource = resource;
        worker.TargetSlot = slot;
        return true;
    }
}
