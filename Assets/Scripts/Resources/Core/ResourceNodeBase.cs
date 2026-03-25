using System;
using UnityEngine;

public abstract class ResourceNodeBase : ValidatedMonoBehaviour, IResourceNode
{
    [SerializeField] protected WorkSlot[] workSlots;

    protected bool available = true;

    public bool IsAvailable => available;

    public abstract float Priority { get; }
    public abstract Vector2 WorkPosition { get; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void Start()
    {
        if (ResourceRegistry.Instance != null)
            ResourceRegistry.Instance.Register(this);
        else
            Debug.LogWarning($"{name}: ResourceRegistry not found", this);
    }

    protected virtual void OnDestroy()
    {
        if (ResourceRegistry.Instance != null)
            ResourceRegistry.Instance.Unregister(this);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.NotEmptyArray(this, workSlots, nameof(workSlots));

        if (workSlots != null)
        {
            for (int i = 0; i < workSlots.Length; i++)
            {
                if (workSlots[i] == null)
                {
                    Debug.LogError($"{name}: workSlots[{i}] is null", this);
                    valid = false;
                }
            }
        }

        return valid;
    }

    public bool HasFreeSlot()
    {
        if (workSlots == null || workSlots.Length == 0)
            return false;

        foreach (var slot in workSlots)
        {
            if (slot != null && slot.IsFree)
                return true;
        }

        return false;
    }

    public virtual WorkSlot TryReserveSlot(Worker worker)
    {
        if (worker == null || workSlots == null)
            return null;

        foreach (var slot in workSlots)
        {
            if (slot == null)
                continue;

            if (slot.TryReserve(worker))
                return slot;
        }

        return null;
    }

    public virtual void ReleaseSlot(Worker worker)
    {
        if (workSlots == null)
            return;

        foreach (var slot in workSlots)
        {
            if (slot != null)
                slot.Release(worker);
        }
    }

    protected abstract void StartWorkRoutine(Action<int> onFinished);

    public virtual bool TryStartWork(Worker worker, Action<int> onFinished)
    {
        if (!available)
            return false;

        if (worker == null)
            return false;

        if (worker.TargetSlot == null)
            return false;

        if (!worker.TargetSlot.IsReservedBy(worker))
            return false;

        available = false;
        StartWorkRoutine(onFinished);
        return true;
    }

    public virtual Vector2 GetWorkPosition(Worker worker)
    {
        if (worker != null &&
            worker.TargetSlot != null &&
            worker.TargetSlot.IsReservedBy(worker))
        {
            return worker.TargetSlot.Position;
        }

        if (workSlots != null)
        {
            foreach (var slot in workSlots)
            {
                if (slot != null)
                    return slot.Position;
            }
        }

        return transform.position;
    }

    public virtual void CancelWork(Worker worker)
    {
        ReleaseSlot(worker);
    }
}