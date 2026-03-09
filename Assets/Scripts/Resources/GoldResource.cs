using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoldResource : ResourceNodeBase
{
    [Header("Timing")]
    [SerializeField] private float mineTime = 3f;
    [SerializeField] private float respawnTime = 15f;
    [SerializeField] private float growInterval = 5f;

    [Header("Visuals")]
    [SerializeField] private Sprite[] sizeSprites; 
    [SerializeField] private Animator animator;

    private SpriteRenderer sr;
    private ResourceSize size = ResourceSize.Tiny;

    public override int Priority => 8;
    public override Vector2 WorkPosition => workSlots[0].Position;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual();
        StartCoroutine(GrowRoutine());
    }

    public override void StartWork(Action<int> onFinished)
    {
        if (!available)
            return;

        available = false;
        StopAllCoroutines();
        StartCoroutine(MineRoutine(onFinished));
    }

    private IEnumerator MineRoutine(Action<int> callback)
    {
        yield return new WaitForSeconds(mineTime);

        int amount = (int)size;
        sr.enabled = false;

        callback?.Invoke(amount);

        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    private void Respawn()
    {
        available = true;
        size = ResourceSize.Tiny;
        sr.enabled = true;
        UpdateVisual();
        StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        while (available)
        {
            yield return new WaitForSeconds(growInterval);

            if (size < ResourceSize.Giant)
            {
                size++;
                UpdateVisual();
            }
        }
    }

    private void UpdateVisual()
    {
        int index = Mathf.Clamp((int)size - 1, 0, sizeSprites.Length - 1);
        animator.SetInteger("Size", index);
    }
}
