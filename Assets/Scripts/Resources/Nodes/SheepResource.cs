using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Овца как ресурс мяса
/// Поддерживает временную заморозку AI, выдачу награды и респавн
/// </summary>
public class SheepResource : ResourceNodeBase
{
    [Header("Config")]
    [SerializeField] private SheepResourceConfig config;

    [Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D col;

    private SheepAI sheepAI;

    public override float Priority => config != null ? config.priority : 1f;
    public override Vector2 WorkPosition => GetWorkPosition(null);

    protected override void Awake()
    {
        base.Awake();
        sheepAI = GetComponent<SheepAI>();

        if (config == null)
            Debug.LogError($"SheepResource {name}: SheepResourceConfig не назначен.", this);
    }

    /// <summary>
    /// Запускает рабочую рутину овцы как ресурса
    /// </summary>
    protected override void StartWorkRoutine(Action<int> onFinished)
    {
        StartCoroutine(WorkRoutine(onFinished));
    }

    /// <summary>
    /// Логика добычи мяса
    /// ожидание, выдача награды, скрытие объекта, ожидание респавна
    /// </summary>
    private IEnumerator WorkRoutine(Action<int> callback)
    {
        float workTime = config != null ? config.workTime : 2f;
        int meatAmount = config != null ? config.meatAmount : 2;
        float respawnTime = config != null ? config.respawnTime : 15f;

        yield return new WaitForSeconds(workTime);

        callback?.Invoke(meatAmount);

        if (sprite != null)
            sprite.enabled = false;

        if (col != null)
            col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        Respawn();
    }

    /// <summary>
    /// Возвращает овцу в доступное состояние после респавна
    /// </summary>
    private void Respawn()
    {
        available = true;

        sprite.enabled = true;
        col.enabled = true;

        sheepAI?.SetFrozen(false);
    }

    /// <summary>
    /// Пытается зарезервировать слот и, если успешно, замораживает овцу
    /// </summary>
    public override WorkSlot TryReserveSlot(Worker worker)
    {
        WorkSlot slot = base.TryReserveSlot(worker);

        if (slot != null)
            sheepAI?.SetFrozen(true);

        return slot;
    }

    /// <summary>
    /// Пытается зарезервировать слот и, если успешно, замораживает овцу
    /// </summary>
    public override void ReleaseSlot(Worker worker)
    {
        base.ReleaseSlot(worker);
        sheepAI?.SetFrozen(false);
    }
}
