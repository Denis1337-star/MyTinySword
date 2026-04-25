using System;
using UnityEngine;

/// <summary>
/// Ѕазова€ логика всех ресурсов
/// </summary>
public abstract class ResourceNodeBase : ValidatedMonoBehaviour, IResourceNode
{
    [SerializeField] private WorkSlot workSlot;

    protected bool available = true;

    public bool IsAvailable => available;

    public abstract float Priority { get; }

    protected virtual void Start()
    {
        if (ResourceRegistry.Instance != null)
            ResourceRegistry.Instance.Register(this);
        else
            Debug.LogWarning($"{name}: ResourceRegistry not found.", this);
    }

    protected virtual void OnDestroy()
    {
        if (ResourceRegistry.Instance != null)
            ResourceRegistry.Instance.Unregister(this);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.NotEmptyCollection(this, workSlot, nameof(workSlot));

        return valid;
    }

    /// <summary>
    /// ѕровер€ет, свободен ли слот ресурса
    /// </summary>
    public bool HasFreeSlot()
    {
        return workSlot != null && workSlot.IsFree;
    }

    /// <summary>
    /// резервирует рабочий слот за worker
    /// </summary>
    public virtual WorkSlot TryReserveSlot(Worker worker)
    {
        if (worker == null || workSlot == null)
            return null;

        return workSlot.TryReserve(worker) ? workSlot : null;
    }

    /// <summary>
    /// ќсвобождает рабочий слот если он принадлежит этому worker
    /// </summary>
    public virtual void ReleaseSlot(Worker worker)
    {
        if (worker == null || workSlot == null)
            return;

        workSlot.Release(worker);
    }

    /// <summary>
    ///  начинает работу на ресурсе
    /// </summary>
    public virtual bool TryStartWork(Worker worker, Action<int> onFinished)
    {
        if (!CanStartWork(worker))
            return false;

        available = false;
        StartWorkRoutine(onFinished);
        return true;
    }

    /// <summary>
    /// ¬озвращает позицию работы дл€ worker
    /// </summary>
    public virtual Vector2 GetWorkPosition(Worker worker)
    {
        if (worker != null &&
            worker.TargetSlot != null &&
            worker.TargetSlot.IsReservedBy(worker))
        {
            return worker.TargetSlot.Position;
        }

        if (workSlot != null)
            return workSlot.Position;

        return transform.position;
    }

    /// <summary>
    /// ќтмен€ет работу worker на ресурсе
    /// </summary>
    public virtual void CancelWork(Worker worker)
    {
        ReleaseSlot(worker);
    }

    /// <summary>
    /// ресурс сам реализует свою рабочую рутину
    /// </summary>
    protected abstract void StartWorkRoutine(Action<int> onFinished);

    private bool CanStartWork(Worker worker)
    {
        if (!available)
            return false;

        if (worker == null)
            return false;

        if (worker.TargetSlot == null)
            return false;

        return worker.TargetSlot.IsReservedBy(worker);
    }
}