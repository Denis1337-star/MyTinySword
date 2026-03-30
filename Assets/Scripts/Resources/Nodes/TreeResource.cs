using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Конкретная реализация дерева как ресурса
/// Поддерживает рубку, визуальное превращение в пень и респавн
/// </summary>
public class TreeResource : ResourceNodeBase
{
    [Header("Config")]
    [SerializeField] private TreeResourceConfig config;

    private Animator animator;

    public override Vector2 WorkPosition => GetWorkPosition(null);
    public override float Priority => config != null ? config.priority : 10f;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        if (config == null)
            Debug.LogError($"TreeResource {name}: TreeResourceConfig не назначен.", this);

        SetTreeVisual();
    }

    // Запускает конкретную рабочую рутину дерева — рубку
    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        StartCoroutine(ChopRoutine(onFinished));
    }

    /// <summary>
    /// Логика рубки дерева
    /// ожидание, выдача награды, смена визуала, ожидание респавна
    /// </summary>
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

    // Возвращает дерево в доступное состояние после респавна
    private void Respawn()
    {
        available = true;
        SetTreeVisual();
    }

    // Устанавливает визуал обычного дерева
    private void SetTreeVisual()
    {
        if (animator != null)
            animator.SetBool("Stump", false);
    }
    // Устанавливает визуал пня после рубки
    private void SetStumpVisual()
    {
        if (animator != null)
            animator.SetBool("Stump", true);
    }
}

