using System;
using UnityEngine;

public abstract class ResourceNodeBase : MonoBehaviour, IResourceNode
{
    [SerializeField] protected WorkSlot[] workSlots;

    protected bool available = true;
    public bool IsAvailable => available;

    public abstract float Priority { get; }
    public abstract Vector2 WorkPosition { get; }

    protected virtual void Start()
    {
        ResourceRegistry.Instance.Register(this);
    }

    protected virtual void OnDestroy()
    {
        if (ResourceRegistry.Instance != null)
            ResourceRegistry.Instance.Unregister(this);
    }

    public virtual WorkSlot GetFreeSlot(Worker worker)
    {
        foreach (var slot in workSlots)
        {
            if (slot.TryReserve(worker))
                return slot;
        }

        return null;
    }

    public virtual void ReleaseSlot(Worker worker)
    {
        foreach (var slot in workSlots)
            slot.Release(worker);
    }

    public abstract void StartWork(Action<int> onFinished);

    public virtual bool TryStartWork(Worker worker, Action<int> onFinished)
    {
        if (!available)
            return false;

        // Работа может стартовать только если рабочий уже имеет слот
        if (worker.TargetSlot == null)
            return false;

        if (!worker.TargetSlot.IsReservedBy(worker))
            return false;

        StartWork(onFinished);
        return true;
    }

    public virtual Vector2 GetWorkPosition(Worker worker)
    {
        if (worker.TargetSlot != null && worker.TargetSlot.IsReservedBy(worker))
            return worker.TargetSlot.Position;

        var slot = GetFreeSlot(worker);
        return slot != null ? slot.Position : (Vector2)transform.position;
    }

    public virtual void CancelWork(Worker worker)
    {
        ReleaseSlot(worker);
    }
}
