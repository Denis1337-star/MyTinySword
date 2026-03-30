using UnityEngine;

/// <summary>
/// Рабочий слот у ресурса
/// Позволяет одному worker зарезервировать точку для подхода и работы
/// </summary>
public class WorkSlot : MonoBehaviour
{
    private Worker reservedBy;  // Worker, который сейчас владеет этим слотом

    public bool IsFree   // Свободен ли слот
    {
        get { return reservedBy == null; }
    }
    public Vector2 Position => transform.position;  // Свободен ли слот.

    /// <summary>
    /// Пытается зарезервировать слот за worker
    /// Возвращает true, если слот свободен или уже принадлежит этому worker
    /// </summary>
    public bool TryReserve(Worker worker)
    {
        if (reservedBy == null)
        {
            reservedBy = worker;
            return true;
        }

        return reservedBy == worker;
    }

    // Проверяет, принадлежит ли слот конкретному worker
    public bool IsReservedBy(Worker worker)
    {
        return reservedBy == worker;
    }

    // Освобождает слот, если его вызывает текущий владелец
    public void Release(Worker worker)
    {
        if (reservedBy == worker)
            reservedBy = null;
    }
}
