using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  реализация золота
/// </summary>
public class GoldResource : ResourceNodeBase
{
    private static readonly int SizeHash = Animator.StringToHash("Size");

    [SerializeField] private GoldResourceConfig _config;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private ResourceSize _size = ResourceSize.Tiny;
    private Coroutine _growRoutine;
    private Coroutine _workRoutine;

    public override float Priority => _config != null ? _config.Priority : 0f;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        base.Awake();

        UpdateVisual();
    }

    protected override void Start()
    {
        base.Start();
        StartGrowRoutine();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));

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

        if (_workRoutine != null)
            StopCoroutine(_workRoutine);

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
        while (_available)
        {
            yield return new WaitForSeconds(_config.GrowInterval);

            if (_size >= ResourceSize.Giant)
                continue;

            _size++;
            UpdateVisual();
        }
    }

    private void Respawn()
    {
        _available = true;
        _size = ResourceSize.Tiny;
        _workRoutine = null;

        SetVisible(true);
        UpdateVisual();
        StartGrowRoutine();
    }

    private void StartGrowRoutine()
    {
        StopGrowRoutine();

        if (_config == null)
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

    private void UpdateVisual()
    {
        int sizeIndex = Mathf.Max(0, (int)_size - 1);

        if (_animator != null)
            _animator.SetInteger(SizeHash, sizeIndex);
    }

    private void SetVisible(bool value)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = value;
    }

    protected override void OnDestroy()
    {
        StopGrowRoutine();

        if (_workRoutine != null)
            StopCoroutine(_workRoutine);

        base.OnDestroy();
    }
}
