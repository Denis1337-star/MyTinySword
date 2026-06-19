/// <summary>
/// Helper для выбора ресурсной точки и рабочего слота для worker.
/// </summary>
public static class WorkerResourceSelector
{
    /// <summary>
    /// Ищет ресурс для текущей job worker.
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
    /// Проверяет, что у worker есть валидное назначение на ресурс.
    /// </summary>
    public static bool HasValidAssignment(Worker worker)
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
