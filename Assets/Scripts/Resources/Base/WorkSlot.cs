using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSlot : MonoBehaviour
{
    private Worker reservedBy;

    public bool IsFree => reservedBy == null;
    public Vector2 Position => transform.position;

    public bool TryReserve(Worker worker)
    {
        if (reservedBy == null)
        {
            reservedBy = worker;
            return true;
        }

        return reservedBy == worker;
    }

    public bool IsReservedBy(Worker worker)
    {
        return reservedBy == worker;
    }

    public void Release(Worker worker)
    {
        if (reservedBy == worker)
            reservedBy = null;
    }
}
