using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Овца как ресурс мяса
/// </summary>
public class SheepResource : ResourceNodeBase
{
    [Header("Config")]
    [SerializeField] private SheepResourceConfig config;

    [Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D col;

    private SheepAI sheepAI;
    private Coroutine workRoutine;

    public override float Priority => config != null ? config.Priority : 1f;

    protected override void Awake()
    {
        sheepAI = GetComponent<SheepAI>();

        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        if (col == null)
            col = GetComponent<Collider2D>();

        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.NotEmptyCollection(this, config, nameof(config));
        valid &= ValidationUtility.NotEmptyCollection(this, sprite, nameof(sprite));
        valid &= ValidationUtility.NotEmptyCollection(this, col, nameof(col));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: SheepResourceConfig is invalid.", this);
            valid = false;
        }

        return valid;
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        if (workRoutine != null)
            StopCoroutine(workRoutine);

        workRoutine = StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> callback)
    {
        yield return new WaitForSeconds(config.WorkTime);

        callback?.Invoke(config.MeatAmount);

        SetVisible(false);

        yield return new WaitForSeconds(config.RespawnTime);

        Respawn();
    }

    public override WorkSlot TryReserveSlot(Worker worker)
    {
        WorkSlot slot = base.TryReserveSlot(worker);

        if (slot != null)
            sheepAI?.SetFrozen(true);

        return slot;
    }

    public override void ReleaseSlot(Worker worker)
    {
        base.ReleaseSlot(worker);

        if (available)
            sheepAI?.SetFrozen(false);
    }

    private void Respawn()
    {
        available = true;
        SetVisible(true);
        sheepAI?.SetFrozen(false);
        workRoutine = null;
    }

    private void SetVisible(bool value)
    {
        if (sprite != null)
            sprite.enabled = value;

        if (col != null)
            col.enabled = value;
    }
}
