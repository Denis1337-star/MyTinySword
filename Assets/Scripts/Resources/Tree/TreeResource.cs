using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : ResourceNodeBase
{
    [Header("Timing")]
    [SerializeField] private float chopTime = 2f;
    [SerializeField] private float respawnTime = 10f;

    [Header("Visuals")]
    [SerializeField] private Sprite treeSprite;
    [SerializeField] private Sprite stumpSprite;

    private SpriteRenderer sr;
    private Animator animator;

    public override Vector2 WorkPosition => workSlots[0].Position;
    public override float Priority => 10f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        SetTreeVisual();
    }

    public override void StartWork(Action<int> onFinished)
    {
        if (!available)
            return;

        available = false;
        StartCoroutine(ChopRoutine(onFinished));
    }

    private IEnumerator ChopRoutine(Action<int> callback)
    {
        yield return new WaitForSeconds(chopTime);

        callback?.Invoke(3);
        SetStumpVisual();

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;
        SetTreeVisual();
    }

    private void SetTreeVisual()
    {
        if (sr != null)
            sr.sprite = treeSprite;

        if (animator != null)
            animator.SetBool("Stump", false);
    }

    private void SetStumpVisual()
    {
        if (sr != null)
            sr.sprite = stumpSprite;

        if (animator != null)
            animator.SetBool("Stump", true);
    }
}

