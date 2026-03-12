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
    public override float Priority => 10;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = treeSprite;
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

        sr.sprite = stumpSprite;
        animator.SetBool("Stump", true);

        callback?.Invoke(3);

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;
        sr.sprite = treeSprite;
        animator.SetBool("Stump", false);
    }
}

