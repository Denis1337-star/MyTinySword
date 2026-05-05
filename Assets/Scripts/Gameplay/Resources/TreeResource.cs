using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  реализация дерева 
/// </summary>
public class TreeResource : ResourceNodeBase
{
    private static readonly int StumpHash = Animator.StringToHash("Stump");

    [SerializeField] private TreeResourceConfig _config;

    private Animator _animator;
    private Coroutine _workRoutine;

    public override float Priority => _config != null ? _config.Priority : 0f;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();

        base.Awake();

        SetTreeVisual();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));

        if (_config != null && !_config.IsValid())
        {
            Debug.LogError($"{name}: TreeResourceConfig некорректный.", this);
            valid = false;
        }

        return valid;
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

        onFinished?.Invoke(_config.RewardAmount);

        SetStumpVisual();

        yield return new WaitForSeconds(_config.RespawnTime);

        Respawn();
    }

    private void Respawn()
    {
        _available = true;
        _workRoutine = null;

        SetTreeVisual();
    }

    private void SetTreeVisual()
    {
        if (_animator != null)
            _animator.SetBool(StumpHash, false);
    }

    private void SetStumpVisual()
    {
        if (_animator != null)
            _animator.SetBool(StumpHash, true);
    }
}

