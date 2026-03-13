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

    public override float Priority => 1;
    public override Vector2 WorkPosition => workSlots[0].Position;

    private void Awake()
    {
        sheepAI = GetComponent<SheepAI>();

        // √ј–јЌ“»я: у овцы ровно 1 слот
        if (workSlots == null || workSlots.Length == 0)
            Debug.LogError("Sheep MUST have exactly 1 WorkSlot");
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
        //  овца уже должна быть заморожена
        yield return new WaitForSeconds(workTime);

        callback?.Invoke(meatAmount);

        spriteRenderer.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    private void Respawn()
    {
        available = true;

        spriteRenderer.enabled = true;
        col.enabled = true;

        sheepAI?.SetFrozen(false);
    }

    public override WorkSlot GetFreeSlot(Worker worker)
    {
        var slot = base.GetFreeSlot(worker);

        if (slot != null)
        {
            // —–ј«” замораживаем овцу при резерве
            sheepAI?.SetFrozen(true);
        }

        return slot;
    }

    public override void ReleaseSlot(Worker worker)
    {
        base.ReleaseSlot(worker);
        sheepAI?.SetFrozen(false);
    }
}
