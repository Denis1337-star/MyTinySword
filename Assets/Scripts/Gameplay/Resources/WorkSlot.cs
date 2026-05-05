using UnityEngine;

/// <summary>
/// –абочий слот у ресурсной точки.
/// ѕозвол€ет одному worker'у зарезервировать конкретную позицию работы.
/// </summary>
public class WorkSlot : MonoBehaviour
{
    private Worker _reservedBy;

    public bool IsFree => _reservedBy == null;
    public Vector2 Position => transform.position;

    /// <summary>
    /// ѕытаетс€ зарезервировать слот за worker'ом.
    /// ≈сли слот уже зан€т этим же worker'ом, возвращает true.
    /// </summary>
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

    /// <summary>
    /// ѕровер€ет, зарезервирован ли слот конкретным worker
    /// </summary>
    public bool IsReservedBy(Worker worker)
    {
        return worker != null && _reservedBy == worker;
    }

    /// <summary>
    /// ќсвобождает слот, если он был зарезервирован этим worker
    /// </summary>
    public void Release(Worker worker)
    {
        if (worker == null)
            return;

        if (_reservedBy != worker)
            return;

        _reservedBy = null;
    }
}
