using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Базовая логика ресурсной точки
/// </summary>
public abstract class ResourceNodeBase : ValidatedMonoBehaviour
{
    [SerializeField] private WorkSlot _workSlot;

    private ResourceRegistry _resourceRegistry;
    private TechTreeBonusService _techTreeBonusService;

    protected bool _isAvailable = true;

    public bool IsAvailable => _isAvailable;

    [Inject]
    private void Construct(
        ResourceRegistry resourceRegistry,
        TechTreeBonusService techTreeBonusService)
    {
        _resourceRegistry = resourceRegistry;
        _techTreeBonusService = techTreeBonusService;
    }

    protected virtual void Start()
    {
        _resourceRegistry.Register(this);
    }

    protected virtual void OnDestroy()
    {
        _resourceRegistry.Unregister(this);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _workSlot, nameof(_workSlot));

        return valid;
    }

    public bool HasFreeSlot()
    {
        return _workSlot != null && _workSlot.IsFree;
    }

    public virtual WorkSlot TryReserveSlot(Worker worker)
    {
        if (worker == null)
            return null;

        return _workSlot.TryReserve(worker) ? _workSlot : null;
    }

    public virtual void ReleaseSlot(Worker worker)
    {
        if (worker == null)
            return;

        _workSlot.Release(worker);
    }

    public virtual bool TryStartWork(Worker worker, Action<int> onFinished)
    {
        if (!CanStartWork(worker))
            return false;

        _isAvailable = false;
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

        return _workSlot.Position;
    }

    public virtual void CancelWork(Worker worker)
    {
        ReleaseSlot(worker);
    }

    public virtual void CompleteWork(Worker worker)
    {
        ReleaseSlot(worker);
    }

    protected void SetAvailable(bool available)
    {
        _isAvailable = available;
    }
    protected float GetWorkTimeWithGatherBonus(float baseWorkTime)
    {

        float finalWorkTime = _techTreeBonusService.ApplyPercentReduction(
            baseWorkTime,
            TechTreeBonusType.WorkersGather);

        return Mathf.Max(0.1f, finalWorkTime);
    }

    protected abstract void StartWorkRoutine(Action<int> onFinished);

    private bool CanStartWork(Worker worker)
    {
        if (!_isAvailable)
            return false;

        if (worker == null)
            return false;

        if (worker.TargetSlot == null)
            return false;

        return worker.TargetSlot.IsReservedBy(worker);
    }
}