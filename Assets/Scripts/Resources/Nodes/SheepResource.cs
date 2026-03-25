using System;
using System.Collections;
using UnityEngine;

public class SheepResource : ResourceNodeBase
{
    [Header("Config")]
    [SerializeField] private SheepResourceConfig config;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D col;

    private SheepAI sheepAI;

    public override float Priority => config != null ? config.priority : 1f;
    public override Vector2 WorkPosition => GetWorkPosition(null);

    protected override void Awake()
    {
        base.Awake();
        sheepAI = GetComponent<SheepAI>();

        if (config == null)
            Debug.LogError($"SheepResource {name}: SheepResourceConfig не назначен.", this);

        if (workSlots == null || workSlots.Length == 0)
            Debug.LogError("SheepResource MUST have at least one WorkSlot", this);
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> callback)
    {
        float workTime = config != null ? config.workTime : 2f;
        int meatAmount = config != null ? config.meatAmount : 2;
        float respawnTime = config != null ? config.respawnTime : 15f;

        yield return new WaitForSeconds(workTime);

        callback?.Invoke(meatAmount);

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (col != null)
            col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (col != null)
            col.enabled = true;

        sheepAI?.SetFrozen(false);
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
        sheepAI?.SetFrozen(false);
    }
}
