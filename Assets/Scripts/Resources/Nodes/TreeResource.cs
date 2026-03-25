using System;
using System.Collections;
using UnityEngine;

public class TreeResource : ResourceNodeBase
{
    [Header("Config")]
    [SerializeField] private TreeResourceConfig config;

    [Header("Visuals")]
    [SerializeField] private Sprite treeSprite;
    [SerializeField] private Sprite stumpSprite;

    private SpriteRenderer sr;
    private Animator animator;

    public override Vector2 WorkPosition => GetWorkPosition(null);
    public override float Priority => config != null ? config.priority : 10f;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (config == null)
            Debug.LogError($"TreeResource {name}: TreeResourceConfig не назначен.", this);

        SetTreeVisual();
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        StartCoroutine(ChopRoutine(onFinished));
    }

    private IEnumerator ChopRoutine(Action<int> callback)
    {
        float chopTime = config != null ? config.chopTime : 2f;
        int rewardAmount = config != null ? config.rewardAmount : 3;
        float respawnTime = config != null ? config.respawnTime : 10f;

        yield return new WaitForSeconds(chopTime);

        callback?.Invoke(rewardAmount);
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

