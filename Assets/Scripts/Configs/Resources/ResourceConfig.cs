using UnityEngine;

/// <summary>
/// Базовый конфиг ресурсов
/// </summary>
public abstract class ResourceConfig : BaseConfig
{
    [SerializeField] private ResourceType _resourceType = ResourceType.None;
    [SerializeField, Min(0f)] private float _respawnTime;
    [SerializeField, Min(0.1f)] private float _workTime;

    public ResourceType ResourceType => _resourceType;
    public float RespawnTime => _respawnTime;
    public float WorkTime => _workTime;

    public override bool IsValid()
    {
        return _resourceType != ResourceType.None &&
          _workTime >= 0.1f &&
          _respawnTime >= 0f;
    }

    protected virtual void OnValidate()
    {
        _workTime = Mathf.Max(0.1f, _workTime);
        _respawnTime = Mathf.Max(0f, _respawnTime);
    }
}