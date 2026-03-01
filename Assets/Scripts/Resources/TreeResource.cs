using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour, IResourceNode
{
    [Header("Positions")]
    [SerializeField] private Transform workPoint;

    [Header("Timing")]
    [SerializeField] private float chopTime = 2f;
    [SerializeField] private float respawnTime = 10f;

    [Header("Visuals")]
    [SerializeField] private Sprite treeSprite;
    [SerializeField] private Sprite stumpSprite;

    private SpriteRenderer sr;
    private Animator animator;

    private bool available = true;
    private Action<int> onFinishedCallback;

    public bool IsAvailable => available;
    public Vector2 WorkPosition => workPoint.position;
    public int Priority => 10;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = treeSprite;
    }

    public void StartWork(Action<int> onFinished)
    {
        if (!available)
            return;

        available = false;
        onFinishedCallback = onFinished;

        CancelInvoke();
        Invoke(nameof(FinishChop), chopTime);
    }

    private void FinishChop()
    {
        sr.sprite = stumpSprite;
        animator.SetBool("Stump", true);
        onFinishedCallback?.Invoke(1);
        Invoke(nameof(Respawn), respawnTime);
    }

    private void Respawn()
    {
        available = true;
        sr.sprite = treeSprite;
        animator.SetBool("Stump", false);
    }
}

