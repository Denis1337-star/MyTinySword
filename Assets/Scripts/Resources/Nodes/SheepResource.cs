using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ресурсная точка овцы
/// </summary>
[RequireComponent(typeof(SheepAI))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public sealed class SheepResource : ResourceNodeBase
{
    [SerializeField] private SheepResourceConfig _config;
    [SerializeField] private SheepAI _sheepAI;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _resourceCollider;

    private Coroutine _workRoutine;

    protected override void Awake()
    {
        base.Awake();

        SetVisible(true);
        SetFrozen(false);
    }

    protected override void OnDestroy()
    {
        StopWorkRoutine();

        base.OnDestroy();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.IsValidConfig(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _sheepAI, nameof(_sheepAI));
        valid &= ValidationUtility.IsAssigned(this, _spriteRenderer, nameof(_spriteRenderer));
        valid &= ValidationUtility.IsAssigned(this, _resourceCollider, nameof(_resourceCollider));

        return valid;
    }

    public override WorkSlot TryReserveSlot(Worker worker)
    {
        WorkSlot slot = base.TryReserveSlot(worker);

        if (slot == null)
            return null;

        SetFrozen(true);

        return slot;
    }

    public override void CancelWork(Worker worker)
    {
        StopWorkRoutine();

        SetAvailable(true);

        base.CancelWork(worker);

        SetVisible(true);
        SetFrozen(false);
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        StopWorkRoutine();

        _workRoutine = StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> onFinished)
    {
        SetFrozen(true);

        yield return new WaitForSeconds(GetWorkTimeWithGatherBonus(_config.WorkTime));

        onFinished?.Invoke(_config.MeatAmount);

        SetVisible(false);
        SetFrozen(true);

        yield return new WaitForSeconds(_config.RespawnTime);

        Respawn();
    }

    private void Respawn()
    {
        SetAvailable(true);
        _workRoutine = null;

        SetVisible(true);
        SetFrozen(false);
    }

    private void SetVisible(bool isVisible)
    {
        _spriteRenderer.enabled = isVisible;
        _resourceCollider.enabled = isVisible;
    }

    private void SetFrozen(bool isFrozen)
    {
        _sheepAI.SetFrozen(isFrozen);
    }

    private void StopWorkRoutine()
    {
        if (_workRoutine == null)
            return;

        StopCoroutine(_workRoutine);
        _workRoutine = null;
    }
}