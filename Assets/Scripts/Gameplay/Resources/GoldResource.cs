using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  реализация золота
/// </summary>
public class GoldResource : ResourceNodeBase
{
    [SerializeField] private GoldResourceConfig config;

    private Animator animator;
    private SpriteRenderer sprite;
    private ResourceSize size = ResourceSize.Tiny;
    private Coroutine growRoutine;
    private Coroutine mineRoutine;

    public override float Priority => config != null ? config.Priority : 8f;

    protected override void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        base.Awake();

        if (!enabled)
            return;

        UpdateVisual();
    }

    protected override void Start()
    {
        base.Start();

        if (!enabled)
            return;

        StartGrowRoutine();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.NotEmptyCollection(this, config, nameof(config));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: GoldResourceConfig is invalid.", this);
            valid = false;
        }

        return valid;
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        StopGrowRoutine();

        if (mineRoutine != null)
            StopCoroutine(mineRoutine);

        mineRoutine = StartCoroutine(MineRoutine(onFinished));
    }

    private IEnumerator MineRoutine(Action<int> callback)
    {
        yield return new WaitForSeconds(config.WorkTime);

        int amount = (int)size;

        if (sprite != null)
            sprite.enabled = false;

        callback?.Invoke(amount);

        yield return new WaitForSeconds(config.RespawnTime);

        Respawn();
    }

    private IEnumerator GrowRoutine()
    {
        while (available)
        {
            yield return new WaitForSeconds(config.GrowInterval);

            if (size >= ResourceSize.Giant)
                continue;

            size++;
            UpdateVisual();
        }
    }

    private void Respawn()
    {
        available = true;
        size = ResourceSize.Tiny;

        if (sprite != null)
            sprite.enabled = true;

        UpdateVisual();
        StartGrowRoutine();

        mineRoutine = null;
    }

    private void StartGrowRoutine()
    {
        StopGrowRoutine();
        growRoutine = StartCoroutine(GrowRoutine());
    }

    private void StopGrowRoutine()
    {
        if (growRoutine == null)
            return;

        StopCoroutine(growRoutine);
        growRoutine = null;
    }

    private void UpdateVisual()
    {
        int index = Mathf.Max(0, (int)size - 1);

        if (animator != null)
            animator.SetInteger("Size", index);
    }
}
