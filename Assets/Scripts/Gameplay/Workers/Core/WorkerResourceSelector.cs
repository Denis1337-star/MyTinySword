/// <summary>
/// Helper дл€ выбора ресурсной точки и рабочего слота дл€ worker
/// </summary>
public static class WorkerResourceSelector
{
    /// <summary>
    /// »щет ресурс дл€ текущей job worker
    /// </summary>
    public static bool TryAssignResourceAndSlot(Worker worker)
    {
        if (worker == null)
            return false;

        worker.ClearCurrentAssignment();

        if (worker.CurrentJobLogic == null)
            return false;

        ResourceNodeBase resource = worker.CurrentJobLogic.FindResource(worker.transform.position);

        if (resource == null)
            return false;

        WorkSlot slot = resource.TryReserveSlot(worker);

        if (slot == null)
        {
            worker.ClearCurrentAssignment();
            return false;
        }

        worker.TargetResource = resource;
        worker.TargetSlot = slot;

        return true;
    }

    /// <summary>
    /// ѕровер€ет можно ли worker двигатьс€ к ресурсу
    /// </summary>
    public static bool HasValidAssignmentForMove(Worker worker)
    {
        if (worker == null)
            return false;

        if (worker.TargetResource == null)
            return false;

        if (worker.TargetSlot == null)
            return false;

        if (!worker.TargetResource.IsAvailable)
            return false;

        return worker.TargetSlot.IsReservedBy(worker);
    }

    /// <summary>
    /// ѕровер€ет можно ли worker начать работу с  ресурсом
    /// </summary>
    public static bool HasValidAssignmentForWork(Worker worker)
    {
        if (worker == null)
            return false;

        if (worker.TargetResource == null)
            return false;

        if (worker.TargetSlot == null)
            return false;

        if (!worker.TargetResource.IsAvailable)
            return false;

        return worker.TargetSlot.IsReservedBy(worker);
    }
}