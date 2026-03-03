using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepResource : ResourceNodeBase
{
    [SerializeField] private int meatAmount = 2;
    [SerializeField] private float workTime = 2f;
    [SerializeField] private float respawnTime = 15f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D col;
    public override Vector2 WorkPosition => transform.position;
    public override int Priority => 1;

    protected override void OnReserved(Worker worker)
    {
        if (TryGetComponent(out SheepAI ai))
            ai.SetFrozen(true); //Œ—“¿Õ¿¬À»¬¿≈Ã —–¿«”
    }

    protected override void OnReleased(Worker worker)
    {
        if (TryGetComponent(out SheepAI ai))
            ai.SetFrozen(false);
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

        spriteRenderer.enabled = false;
        col.enabled = false;

        reservedBy = null;

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;
        reservedBy = null;

        if (TryGetComponent(out SheepAI ai))
            ai.SetFrozen(false);

        spriteRenderer.enabled = true;
        col.enabled = true;
    }
}
