using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoldResource : MonoBehaviour, IResourceNode
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
    private bool available = true;
    private Action<int> onFinishedCallback;

    private ResourceSize size = ResourceSize.Tiny;

    public bool IsAvailable => available;
    public Vector2 WorkPosition => workPoint.position;
    public int Priority => 10;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual();
        InvokeRepeating(nameof(Grow), growInterval, growInterval);
    }

    public void StartWork(Action<int> onFinished)
    {
        if (!available)
            return;

        available = false;
        onFinishedCallback = onFinished;
        CancelInvoke(nameof(Grow));

        Invoke(nameof(FinishMine), mineTime);

    }
    private void FinishMine()
    {
        sr.enabled = false; // камень исчез
       onFinishedCallback?.Invoke((int)size);
        Invoke(nameof(Respawn), respawnTime);
    }

    private void Respawn()
    {
        size = ResourceSize.Tiny;
        available = true;
        sr.enabled = true;
        UpdateVisual();
        InvokeRepeating(nameof(Grow), growInterval, growInterval);
    }

    private void Grow()
    {
        if (!available)
            return;

        if (size < ResourceSize.Giant)
        {
            size++;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        int index = (int)size - 1;
        sr.sprite = sizeSprites[index];
        animator.SetInteger("Size", index);
    }
}
