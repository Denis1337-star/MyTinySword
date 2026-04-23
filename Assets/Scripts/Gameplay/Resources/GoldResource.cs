using System;
using System.Collections;
using UnityEngine;

/// <summary>
///  онкретна€ реализаци€ золотого ресурса
/// «олото растЄт со временем, а размер вли€ет на награду
/// </summary>
public class GoldResource : ResourceNodeBase
{
    [Header("Config")]
    [SerializeField] private GoldResourceConfig config;

    private Animator animator;
    private SpriteRenderer sprite;
    private ResourceSize size = ResourceSize.Tiny;
    private Coroutine growRoutine;

    public override float Priority => config != null ? config.Priority : 8f;
    public override Vector2 WorkPosition => GetWorkPosition(null);

    protected override void Awake()
    {
        base.Awake();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (config == null)
            Debug.LogError($"GoldResource {name}: GoldResourceConfig не назначен.", this);

        UpdateVisual();
    }

    protected override void Start()
    {
        base.Start();
        growRoutine = StartCoroutine(GrowRoutine());
    }

    /// <summary>
    /// «апускает рутину добычи золота
    /// ѕеред началом добычи останавливает рост ресурса
    /// </summary>
    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        if (growRoutine != null)
            StopCoroutine(growRoutine);

        StartCoroutine(MineRoutine(onFinished));
    }

    /// <summary>
    /// «апускает рутину добычи золота
    /// ѕеред началом добычи останавливает рост ресурса
    /// </summary>
    private IEnumerator MineRoutine(Action<int> callback)
    {
        float mineTime = config != null ? config.MineTime : 3f;
        float respawnTime = config != null ? config.RespawnTime : 15f;

        yield return new WaitForSeconds(mineTime);

        int amount = (int)size;

        if (sprite != null)
            sprite.enabled = false;

        callback?.Invoke(amount);

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    // ¬озвращает золото в исходное состо€ние и снова запускает рост
    private void Respawn()
    {
        available = true;
        size = ResourceSize.Tiny;

        sprite.enabled = true;
        UpdateVisual();
        growRoutine = StartCoroutine(GrowRoutine());
    }

    // ѕостепенно увеличивает размер золота, пока ресурс доступен
    private IEnumerator GrowRoutine()
    {
        float growInterval = config != null ? config.GrowInterval : 5f;

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

    // ѕостепенно увеличивает размер золота, пока ресурс доступен
    private void UpdateVisual()
    {
        int index = Mathf.Max(0, (int)size - 1);

        if (animator != null)
            animator.SetInteger("Size", index);
    }
}
