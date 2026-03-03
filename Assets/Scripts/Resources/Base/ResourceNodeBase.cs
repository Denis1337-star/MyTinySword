using System;
using UnityEngine;

public abstract class ResourceNodeBase : MonoBehaviour, IResourceNode
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
        OnReserved(worker);
        return true;
    }

    public void Release(Worker worker)
    {
        if (reservedBy != worker)
            return;

        reservedBy = null;
        OnReleased(worker);
    }

    protected virtual void OnReserved(Worker worker) { }
    protected virtual void OnReleased(Worker worker) { }
    public abstract void StartWork(Action<int> onFinished);
}
