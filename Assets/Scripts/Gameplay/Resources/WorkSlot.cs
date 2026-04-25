using UnityEngine;

/// <summary>
/// Рабочий слот у ресурса
/// </summary>
public class WorkSlot : MonoBehaviour
{
    private Worker reservedBy;

    public bool IsFree => reservedBy == null;

    public Vector2 Position => transform.position;

    /// <summary>
    /// Пытается зарезервировать слот за worker
    /// </summary>
    public bool TryReserve(Worker worker)
    {
        if (worker == null)
            return false;

        if (reservedBy == null)
        {
            reservedBy = worker;
            return true;
        }

        return reservedBy == worker;
    }

    /// <summary>
    /// Проверяет, принадлежит ли слот конкретному worker
    /// </summary>
    public bool IsReservedBy(Worker worker)
    {
        return worker != null && reservedBy == worker;
    }

    /// <summary>
    /// Освобождает слот
    /// </summary>
    public void Release(Worker worker)
    {
        if (worker == null)
            return;

        if (reservedBy == worker)
            reservedBy = null;
    }
}
