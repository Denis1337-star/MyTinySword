using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSlot : MonoBehaviour
{
    private Worker reservedBy;
    public bool IsFree => reservedBy == null;
    public bool TryReserve(Worker worker)
    {
        if (reservedBy != null)
            return false;

        reservedBy = worker;
        return true;
    }

    public void Release(Worker worker)
    {
        if (reservedBy == worker)
            reservedBy = null;
    }

    public Vector2 Position => transform.position;
}
