using System;
using System.Collections;
using UnityEngine;


public class GoldResource : ResourceNodeBase
{
    [Header("Config")]
    [SerializeField] private GoldResourceConfig config;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    private SpriteRenderer sr;
    private ResourceSize size = ResourceSize.Tiny;
    private Coroutine growRoutine;

    public override float Priority => config != null ? config.priority : 8f;
    public override Vector2 WorkPosition => GetWorkPosition(null);

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();

        if (config == null)
            Debug.LogError($"GoldResource {name}: GoldResourceConfig не назначен.", this);

        UpdateVisual();
    }

    protected override void Start()
    {
        base.Start();
        growRoutine = StartCoroutine(GrowRoutine());
    }

    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        if (growRoutine != null)
            StopCoroutine(growRoutine);

        StartCoroutine(MineRoutine(onFinished));
    }

    private IEnumerator MineRoutine(Action<int> callback)
    {
        float mineTime = config != null ? config.mineTime : 3f;
        float respawnTime = config != null ? config.respawnTime : 15f;

        yield return new WaitForSeconds(mineTime);

        int amount = (int)size;

        if (sr != null)
            sr.enabled = false;

        callback?.Invoke(amount);

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    private void Respawn()
    {
        available = true;
        size = ResourceSize.Tiny;

        if (sr != null)
            sr.enabled = true;

        UpdateVisual();
        growRoutine = StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        float growInterval = config != null ? config.growInterval : 5f;

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
        int index = Mathf.Max(0, (int)size - 1);

        if (animator != null)
            animator.SetInteger("Size", index);
    }
}
