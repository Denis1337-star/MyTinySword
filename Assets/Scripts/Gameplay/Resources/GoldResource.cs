using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ресурсная точка золота
/// </summary>
public sealed class GoldResource : ResourceNodeBase
{
    private static readonly int SizeHash = Animator.StringToHash("Size");

    [SerializeField] private GoldResourceConfig _config;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private ResourceSize _size = ResourceSize.Tiny;
    private Coroutine _growRoutine;
    private Coroutine _workRoutine;


    protected override void Awake()
    {
        ResolveReferences();

        base.Awake();

        UpdateVisual();
    }

    protected override void Start()
    {
        base.Start();

        StartGrowRoutine();
    }

    protected override void OnDestroy()
    {
        StopGrowRoutine();
        StopWorkRoutine();

        base.OnDestroy();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _animator, nameof(_animator));
        valid &= ValidationUtility.IsAssigned(this, _spriteRenderer, nameof(_spriteRenderer));

        if (_config != null && !_config.IsValid())
        {
            Debug.LogError($"{name}: GoldResourceConfig некорректный.", this);
            valid = false;
        }

        return valid;
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        StopGrowRoutine();
        StopWorkRoutine();

        if (_config == null || !_config.IsValid())
        {
            SetAvailable(true);
            onFinished?.Invoke(0);
            return;
        }

        _workRoutine = StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> onFinished)
    {
        yield return new WaitForSeconds(_config.WorkTime);

        int amount = Mathf.Max(1, (int)_size);

        SetVisible(false);

        onFinished?.Invoke(amount);

        yield return new WaitForSeconds(_config.RespawnTime);

        Respawn();
    }

    private IEnumerator GrowRoutine()
    {
        while (IsAvailable)
        {
            yield return new WaitForSeconds(_config.GrowInterval);

            if (_size >= ResourceSize.Giant)
                continue;

            _size++;
            UpdateVisual();
        }

        _growRoutine = null;
    }

    private void Respawn()
    {
        SetAvailable(true);

        _size = ResourceSize.Tiny;
        _workRoutine = null;

        SetVisible(true);
        UpdateVisual();
        StartGrowRoutine();
    }

    private void StartGrowRoutine()
    {
        StopGrowRoutine();

        if (_config == null || !_config.IsValid())
            return;

        if (!IsAvailable)
            return;

        _growRoutine = StartCoroutine(GrowRoutine());
    }

    private void StopGrowRoutine()
    {
        if (_growRoutine == null)
            return;

        StopCoroutine(_growRoutine);
        _growRoutine = null;
    }

    private void StopWorkRoutine()
    {
        if (_workRoutine == null)
            return;

        StopCoroutine(_workRoutine);
        _workRoutine = null;
    }

    private void UpdateVisual()
    {
        int sizeIndex = Mathf.Max(0, (int)_size - 1);

            _animator.SetInteger(SizeHash, sizeIndex);
    }

    private void SetVisible(bool value)
    {
            _spriteRenderer.enabled = value;
    }

    private void ResolveReferences()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }
}