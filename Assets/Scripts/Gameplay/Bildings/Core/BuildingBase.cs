using UnityEngine;

/// <summary>
/// Базовый класс для всех зданий
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

    public string DisplayName =>  config.DisplayName;
    public FactionType Faction =>  factionMember.Faction;

    private void OnValidate()
    {
        ResolveReferences();
    }

    protected override void Awake()
    {
        ResolveReferences();
        ApplyConfig();

        base.Awake();
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

        valid &= ValidationUtility.IsAssigned(this, config, nameof(config));
        valid &= ValidationUtility.IsAssigned(this, factionMember, nameof(factionMember));
        valid &= ValidationUtility.IsAssigned(this, health, nameof(health));
        valid &= ValidationUtility.IsAssigned(this, selectable, nameof(selectable));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: BuildingConfig is invalid.", this);
            valid = false;
        }

        return valid;
    }

    protected virtual void HandleDeath()
    {
        Destroy(gameObject);
    }

    private void ResolveReferences()
    {
        if (factionMember == null)
            factionMember = GetComponent<FactionMember>();

        if (health == null)
            health = GetComponent<Health>();

        if (selectable == null)
            selectable = GetComponent<UnitSelectable>();
    }
    private void ApplyConfig()
    {
        if (config == null || health == null)
            return;

        health.Initialize(config.MaxHealth);
    }
}