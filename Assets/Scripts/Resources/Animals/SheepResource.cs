using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepResource : ResourceNodeBase
{
    [Header("Reward")]
    [SerializeField] private int meatAmount = 2;

    [Header("Timing")]
    [SerializeField] private float workTime = 2f;
    [SerializeField] private float respawnTime = 15f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D col;

    private SheepAI sheepAI;

    public override float Priority => 1f;
    public override Vector2 WorkPosition => workSlots[0].Position;

    private void Awake()
    {
        sheepAI = GetComponent<SheepAI>();

        if (workSlots == null || workSlots.Length == 0)
            Debug.LogError("SheepResource MUST have at least one WorkSlot", this);
    }

    public override void StartWork(Action<int> onFinished)
    {
        if (!available)
            return;

        available = false;
        StartCoroutine(WorkRoutine(onFinished));
    }

    private IEnumerator WorkRoutine(Action<int> callback)
    {
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
