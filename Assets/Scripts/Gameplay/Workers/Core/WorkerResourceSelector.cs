
/// <summary>
/// Отвечает за выбор ресурса для worker
/// </summary>
public static class WorkerResourceSelector
{
    /// <summary>
    /// Находит лучший ресурс для текущей job-логики worker
    /// </summary>
    public static ResourceNodeBase FindBestResource(Worker worker)
    {
        if (worker == null)
            return null;

        if (worker.CurrentJobLogic == null)
            return null;

        return worker.CurrentJobLogic.FindResource(worker.transform.position);
    }

    /// <summary>
    /// Пытается зарезервировать свободный слот на выбранном ресурсе
    /// </summary>
    public static WorkSlot TryReserveSlot(Worker worker, ResourceNodeBase resource)
    {
        if (worker == null)
            return null;

        if (resource == null)
            return null;

        if (!resource.IsAvailable)
            return null;

        if (!resource.HasFreeSlot())
            return null;

        return resource.TryReserveSlot(worker);
    }

    /// <summary>
    /// Находит ресурс, резервирует слот и записывает их в worker
    /// </summary>
    public static bool TryAssignResourceAndSlot(Worker worker)
    {
        if (worker == null)
            return false;

        if (worker.CurrentJobLogic == null)
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
    /// Проверяет, что назначение worker всё ещё валидно во время движения к ресурсу
    /// </summary>
    public static bool HasValidAssignmentForMove(Worker worker)
    {
        if (!HasReservedSlot(worker))
            return false;

        return worker.TargetResource.IsAvailable;
    }

    /// <summary>
    /// Проверяет, что назначение worker всё ещё валидно во время работы
    /// </summary>
    public static bool HasValidAssignmentForWork(Worker worker)
    {
        return HasReservedSlot(worker);
    }

    /// <summary>
    /// Проверяет, что у worker есть ресурс и слот, а слот зарезервирован именно этим worker
    /// </summary>
    private static bool HasReservedSlot(Worker worker)
    {
        if (worker == null)
            return false;

        if (worker.TargetResource == null)
            return false;

        if (worker.TargetSlot == null)
            return false;

        return worker.TargetSlot.IsReservedBy(worker);
    }
}
