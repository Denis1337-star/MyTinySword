using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ресурсная точка дерева
/// </summary>
[RequireComponent(typeof(Animator))]
public sealed class TreeResource : ResourceNodeBase
{
    private static readonly int StumpHash = Animator.StringToHash("Stump");

    [SerializeField] private TreeResourceConfig _config;

    private Animator _animator;
    private Coroutine _workRoutine;

    protected override void Awake()
    {
        ResolveReferences();

        base.Awake();

        SetTreeVisual();
    }

    protected override void OnDestroy()
    {
        StopWorkRoutine();

        base.OnDestroy();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _animator, nameof(_animator));

        if (_config != null && !_config.IsValid())
        {
            Debug.LogError($"{name}: TreeResourceConfig некорректный.", this);
            valid = false;
        }

        return valid;
    }

    /// <summary>
    /// Отменяет добычу дерева
    /// </summary>
    public override void CancelWork(Worker worker)
    {
        StopWorkRoutine();

        SetAvailable(true);

        base.CancelWork(worker);

        SetTreeVisual();
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
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

        onFinished?.Invoke(_config.RewardAmount);

        SetStumpVisual();

        yield return new WaitForSeconds(_config.RespawnTime);

        Respawn();
    }

    private void Respawn()
    {
        SetAvailable(true);
        _workRoutine = null;

        SetTreeVisual();
    }

    private void SetTreeVisual()
    {
        _animator.SetBool(StumpHash, false);
    }

    private void SetStumpVisual()
    {
        _animator.SetBool(StumpHash, true);
    }

    private void StopWorkRoutine()
    {
        if (_workRoutine == null)
            return;

        StopCoroutine(_workRoutine);
        _workRoutine = null;
    }

    private void ResolveReferences()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }
}