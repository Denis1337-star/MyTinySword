using System;
using UnityEngine;

public abstract class ResourceNodeBase : MonoBehaviour, IResourceNode
{
    [SerializeField] protected WorkSlot[] workSlots;
    protected bool available = true;

    public bool IsAvailable => available;

    public abstract int Priority { get; }
    public abstract Vector2 WorkPosition { get; }

    public virtual WorkSlot GetFreeSlot(Worker worker)
    {
        foreach (var slot in workSlots)
        {
            if (slot.TryReserve(worker))
                return slot;
        }
        return null;
    }

    public virtual  void ReleaseSlot(Worker worker)
    {
        foreach (var slot in workSlots)
            slot.Release(worker);
    }

    public abstract void StartWork(Action<int> onFinished);
}
