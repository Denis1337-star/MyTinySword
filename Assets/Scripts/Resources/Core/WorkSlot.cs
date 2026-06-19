using UnityEngine;

/// <summary>
/// Рабочий слот у ресурсной точки
/// </summary>
public sealed class WorkSlot : MonoBehaviour
{
    private Worker _reservedBy;

    public bool IsFree => _reservedBy == null;
    public Vector2 Position => transform.position;

    public bool TryReserve(Worker worker)
    {
        if (worker == null)
            return false;

        if (_reservedBy == null)
        {
            _reservedBy = worker;
            return true;
        }

        return _reservedBy == worker;
    }

    public bool IsReservedBy(Worker worker)
    {
        return worker != null && _reservedBy == worker;
    }

    public void Release(Worker worker)
    {
        if (worker == null)
            return;

        if (_reservedBy != worker)
            return;

        _reservedBy = null;
    }
}