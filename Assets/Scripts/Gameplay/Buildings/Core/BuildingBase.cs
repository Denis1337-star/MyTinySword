using UnityEngine;
using Zenject;

/// <summary>
/// Базовый класс для всех зданий
/// </summary>
[RequireComponent(typeof(FactionMember))]
[RequireComponent(typeof(UnitSelectable))]
public abstract class BuildingBase : ValidatedMonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected BuildingConfig config;

    [Header("Components")]
    [SerializeField] protected FactionMember factionMember;
    [SerializeField] protected Health health;
    [SerializeField] protected UnitSelectable selectable;

    private BuildingRegistry _buildingRegistry;
    private GameAudioService _audioService;
    private ConstructionSlot _sourceSlot;

    private bool _isDestroying;

    public BuildingConfig Config => config;
    public FactionMember FactionMember => factionMember;
    public Health Health => health;
    public UnitSelectable Selectable => selectable;

    public string DisplayName => config.DisplayName;
    public FactionType Faction => factionMember.Faction;

    [Inject]
    private void Construct(
        BuildingRegistry buildingRegistry,
        GameAudioService audioService)
    {
        _buildingRegistry = buildingRegistry;
        _audioService = audioService;
    }

    protected override void Awake()
    {
        ApplyConfig();

        base.Awake();
    }

    protected virtual void OnEnable()
    {
            health.OnDied += HandleDeath;
    }

    protected virtual void OnDisable()
    {
            health.OnDied -= HandleDeath;
    }
    protected virtual void OnValidate()
    {
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
            Debug.LogError($"{name}: BuildingConfig настроен некорректно.", this);
            valid = false;
        }

        return valid;
    }


    public void AttachConstructionSlot(ConstructionSlot slot)
    {
        _sourceSlot = slot;
    }

    public void Demolish()
    {
        PlayDemolishSound();
        HandleDeath();
    }

    protected virtual void HandleDeath()
    {
        if (_isDestroying)
            return;

        _isDestroying = true;

        UnregisterUniqueBuilding();
        RestoreSourceSlot();

        Destroy(gameObject);
    }


    private void ApplyConfig()
    {

        health.Initialize(config.MaxHealth);
    }

    private void PlayDemolishSound()
    {
        _audioService.PlayWorldSound(SoundId.BuildingDemolished, transform.position);
    }

    private void UnregisterUniqueBuilding()
    {

        if (!config.UniqueBuilding)
            return;

        _buildingRegistry.UnregisterBuilt(config);
    }

    private void RestoreSourceSlot()
    {
        if (_sourceSlot == null)
            return;

        _sourceSlot.Restore();
        _sourceSlot = null;
    }
}