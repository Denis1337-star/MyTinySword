
/// <summary>
/// ќтвечает за выбор ресурса дл€ worker
/// </summary>
public static class WorkerResourceSelector
{
    public static ResourceNodeBase FindBestResource(Worker worker)
    {
        if (worker == null || worker.CurrentJobLogic == null)
            return null;

        return worker.CurrentJobLogic.FindResource(worker.transform.position);
    }

    public static WorkSlot TryReserveSlot(Worker worker, ResourceNodeBase resource)
    {
        if (worker == null || resource == null)
            return null;

        if (!resource.IsAvailable)
            return null;

        if (!resource.HasFreeSlot())
            return null;

        return resource.TryReserveSlot(worker);
    }

    public static bool TryAssignResourceAndSlot(Worker worker)
    {
        if (worker == null || worker.CurrentJobLogic == null)
            return false;

        worker.ClearCurrentAssignment();

        ResourceNodeBase resource = FindBestResource(worker);
        if (resource == null)
            return false;

        WorkSlot slot = TryReserveSlot(worker, resource);
        if (slot == null)
            return false;

        worker.TargetResource = resource;
        worker.TargetSlot = slot;
        return true;
    }

    /// <summary>
    /// ѕровер€ет назначение worker на этапе движени€ к ресурс
    /// </summary>
    public static bool HasValidAssignmentForMove(Worker worker)
    {
        if (!HasReservedSlot(worker))
            return false;

        return worker.TargetResource.IsAvailable;
    }

    /// <summary>
    /// ѕровер€ет назначение worker во врем€ работы
    /// </summary>
    public static bool HasValidAssignmentForWork(Worker worker)
    {
        return HasReservedSlot(worker);
    }

    private static bool HasReservedSlot(Worker worker)
    {
        if (worker == null)
            return false;

        if (worker.TargetResource == null || worker.TargetSlot == null)
            return false;

        return worker.TargetSlot.IsReservedBy(worker);
    }
}
