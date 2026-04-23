using System;
using UnityEngine;

/// <summary>
/// Базовый класс для всех ресурсов, с которыми могут работать worker'ы.
/// Содержит общую логику слотов, доступности и регистрации в ResourceRegistry
/// </summary>
public abstract class ResourceNodeBase : ValidatedMonoBehaviour, IResourceNode
{
    [SerializeField] protected WorkSlot[] workSlots;

    protected bool available = true;  // Доступен ли ресурс для работы в данный момент

    public bool IsAvailable => available;

    public abstract float Priority { get; }
    public abstract Vector2 WorkPosition { get; }

    protected override void Awake()
    {
        base.Awake();
    }

    // Регистрирует ресурс в общем реестре ресурсов сцены
    protected virtual void Start()
    {
        if (ResourceRegistry.Instance != null)
            ResourceRegistry.Instance.Register(this);
        else
            Debug.LogWarning($"{name}: ResourceRegistry not found", this);
    }

    // Удаляет ресурс из реестра при уничтожении
    protected virtual void OnDestroy()
    {
        if (ResourceRegistry.Instance != null)
            ResourceRegistry.Instance.Unregister(this);
    }

    // Удаляет ресурс из реестра при уничтожении
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

    // Есть ли у ресурса хотя бы один свободный слот
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

    // Пытается зарезервировать слот для worker
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

    // Освобождает слот(ы), принадлежащие указанному worker
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

    /// <summary>
    /// Запускает конкретную рабочую рутину ресурса.
    /// Реализуется в наследниках.
    /// </summary>
    protected abstract void StartWorkRoutine(Action<int> onFinished);

    // Пытается начать работу на ресурсе
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

    // Возвращает рабочую позицию для конкретного worker или fallback-позицию
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

    /// <summary>
    /// Отменяет работу worker на ресурсе
    /// По умолчанию просто освобождает слот
    /// </summary>
    public virtual void CancelWork(Worker worker)
    {
        ReleaseSlot(worker);
    }
}