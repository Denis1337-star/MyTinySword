using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoldResource : ResourceNodeBase
{
    [Header("Positions")]
    [SerializeField] private Transform workPoint;

    [Header("Timing")]
    [SerializeField] private float mineTime = 3f;
    [SerializeField] private float respawnTime = 15f;
    [SerializeField] private float growInterval = 5f;

    [Header("Visuals")]
    [SerializeField] private Sprite[] sizeSprites; // 0 = Tiny ... 5 = Giant
    [SerializeField] private Animator animator;

    private SpriteRenderer sr;
    private ResourceSize size = ResourceSize.Tiny;
    public override Vector2 WorkPosition => workPoint.position;
    public override int Priority => 8;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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

        sr.enabled = false;
        callback?.Invoke((int)size);

        reservedBy = null;

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
        int index = (int)size - 1;
        sr.sprite = sizeSprites[index];
        animator.SetInteger("Size", index);
    }
}
