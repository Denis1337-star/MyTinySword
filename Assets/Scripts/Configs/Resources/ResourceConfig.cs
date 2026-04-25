using UnityEngine;

/// <summary>
/// Базовый конфиг для всех ресурсов
/// </summary>
public abstract class ResourceConfig : BaseConfig
{
    [Header("Common")]
    [Min(0f)]
    [SerializeField] private float priority = 1f;

    [Min(0.1f)]
    [SerializeField] private float respawnTime;

    [Min(0.1f)]
    [SerializeField] private float workTime;


    public float Priority => priority;
    public float RespawnTime => respawnTime;
    public float WorkTime => workTime;

    /// <summary>
    /// Ограничивает общие значения ресурса в редакторе
    /// </summary>
    protected virtual void OnValidate()
    {
        priority = Mathf.Max(0f, priority);
        respawnTime = Mathf.Max(0.1f, respawnTime);
        workTime = Mathf.Max(1f, workTime);
    }
}