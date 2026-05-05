using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Овца как ресурс мяса
/// </summary>
public class SheepResource : ResourceNodeBase
{
    [SerializeField] private SheepResourceConfig _config;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    private SheepAI _sheepAI;
    private Coroutine _workRoutine;

    public override float Priority => _config != null ? _config.Priority : 0f;

    protected override void Awake()
    {
        _sheepAI = GetComponent<SheepAI>();

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _spriteRenderer, nameof(_spriteRenderer));
        valid &= ValidationUtility.IsAssigned(this, _collider, nameof(_collider));

        if (_config != null && !_config.IsValid())
        {
            Debug.LogError($"{name}: SheepResourceConfig некорректный.", this);
            valid = false;
        }

        return valid;
    }

    public override WorkSlot TryReserveSlot(Worker worker)
    {
        WorkSlot slot = base.TryReserveSlot(worker);

        if (slot != null)
            _sheepAI?.SetFrozen(true);

        return slot;
    }

    public override void ReleaseSlot(Worker worker)
    {
        base.ReleaseSlot(worker);

        if (_available)
            _sheepAI?.SetFrozen(false);
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        if (_workRoutine != null)
            StopCoroutine(_workRoutine);

        _workRoutine = StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> onFinished)
    {
        yield return new WaitForSeconds(_config.WorkTime);

        onFinished?.Invoke(_config.MeatAmount);

        SetVisible(false);

        yield return new WaitForSeconds(_config.RespawnTime);

        Respawn();
    }

    private void Respawn()
    {
        _available = true;
        _workRoutine = null;

        SetVisible(true);
        _sheepAI?.SetFrozen(false);
    }

    private void SetVisible(bool value)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = value;

        if (_collider != null)
            _collider.enabled = value;
    }

    protected override void OnDestroy()
    {
        if (_workRoutine != null)
            StopCoroutine(_workRoutine);

        base.OnDestroy();
    }
}
