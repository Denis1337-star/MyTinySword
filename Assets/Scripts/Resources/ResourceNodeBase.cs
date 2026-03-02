using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceNodeBase : MonoBehaviour,IResourceNode
{
    protected bool available = true;
    protected Worker reservedBy;

    public abstract Vector2 WorkPosition { get; }
    public abstract int Priority { get; }

    public bool IsAvailable => available && reservedBy == null;

    public bool TryReserve(Worker worker)
    {
        if (!IsAvailable)
            return false;

        reservedBy = worker;
        return true;
    }

    public void Release(Worker worker)
    {
        if (reservedBy == worker)
            reservedBy = null;
    }

    public abstract void StartWork(Action<int> onFinished);
}
