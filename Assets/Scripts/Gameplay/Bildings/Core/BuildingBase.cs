using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый класс для всех зданий.
/// Содержит общую конфигурацию, фракцию, здоровье
/// и базовую реакцию на уничтожение.
/// </summary>
public class BuildingBase : ValidatedMonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected BuildingConfig config;

    [Header("Components")]
    [SerializeField] protected FactionMember factionMember;
    [SerializeField] protected Health health;
    [SerializeField] protected UnitSelectable selectable;

    public BuildingConfig Config => config;
    public FactionMember FactionMember => factionMember;
    public Health Health => health;
    public UnitSelectable Selectable => selectable;

    public string DisplayName => config != null ? config.DisplayName : name;
    public FactionType Faction => factionMember != null ? factionMember.Faction : FactionType.Neutral;

    private void OnValidate()
    {
        if (factionMember == null)
            factionMember = GetComponent<FactionMember>();

        if (health == null)
            health = GetComponent<Health>();

        if (selectable == null)
            selectable = GetComponent<UnitSelectable>();
    }

    protected virtual void OnEnable()
    {
        if (health != null)
            health.OnDied += HandleDeath;
    }

    protected virtual void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.NotEmptyCollection(this, config, nameof(config));
        valid &= ValidationUtility.NotEmptyCollection(this, factionMember, nameof(factionMember));
        valid &= ValidationUtility.NotEmptyCollection(this, health, nameof(health));
        valid &= ValidationUtility.NotEmptyCollection(this, selectable, nameof(selectable));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: BuildingConfig is invalid", this);
            valid = false;
        }

        return valid;
    }

    /// <summary>
    /// Базовая реакция на уничтожение здания.
    /// Пока просто удаляем объект.
    /// Позже сюда добавим VFX, SFX, анимацию разрушения и игровые события.
    /// </summary>
    protected virtual void HandleDeath()
    {
        Destroy(gameObject);
    }
}