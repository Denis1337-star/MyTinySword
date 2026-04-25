using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  реализация дерева 
/// </summary>
public class TreeResource : ResourceNodeBase
{
    [SerializeField] private TreeResourceConfig config;

    private Animator animator;
    private Coroutine workRoutine;
    public override float Priority => config != null ? config.Priority : 10f;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();

        base.Awake();

        if (!enabled)
            return;

        SetTreeVisual();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.NotEmptyCollection(this, config, nameof(config));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: TreeResourceConfig is invalid.", this);
            valid = false;
        }

        return valid;
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        if (workRoutine != null)
            StopCoroutine(workRoutine);

        workRoutine = StartCoroutine(ChopRoutine(onFinished));
    }

    private IEnumerator ChopRoutine(Action<int> callback)
    {
        yield return new WaitForSeconds(config.WorkTime);

        callback?.Invoke(config.RewardAmount);
        SetStumpVisual();

        yield return new WaitForSeconds(config.RespawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;
        SetTreeVisual();
        workRoutine = null;
    }

    private void SetTreeVisual()
    {
        if (animator != null)
            animator.SetBool("Stump", false);
    }

    private void SetStumpVisual()
    {
        if (animator != null)
            animator.SetBool("Stump", true);
    }
}

