using UnityEngine;
using Zenject;

/// <summary>
/// Базовый класс для всех зданий.
/// Отвечает за конфиг, здоровье, фракцию, выбор, снос и разрушение здания.
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

    [Header("Demolish")]
    [SerializeField] private bool _canBeDemolishedByButton = true;

    private BuildingRegistry _buildingRegistry;
    private GameAudioService _audioService;
    private ConstructionSlot _sourceSlot;
    private TechTreeBonusService _techTreeBonusService;

    private bool _isDestroying;

    public BuildingConfig Config => config;
    public FactionMember FactionMember => factionMember;
    public Health Health => health;
    public UnitSelectable Selectable => selectable;

    public string DisplayName => config.DisplayName;
    public FactionType Faction => factionMember.Faction;

    /// <summary>
    /// Можно ли снести это здание через UI-кнопку.
    /// Не влияет на уничтожение здания юнитами через Health.
    /// </summary>
    public bool CanBeDemolishedByButton => _canBeDemolishedByButton;

    [Inject]
    private void Construct(
        BuildingRegistry buildingRegistry,
        GameAudioService audioService,
        TechTreeBonusService techTreeBonusService)
    {
        _buildingRegistry = buildingRegistry;
        _audioService = audioService;
        _techTreeBonusService = techTreeBonusService;
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

    /// <summary>
    /// Сносит здание напрямую.
    /// Используется внутренней логикой здания.
    /// Для UI-кнопки лучше использовать TryDemolishByButton().
    /// </summary>
    public void Demolish()
    {
        PlayDemolishSound();
        HandleDeath();
    }

    /// <summary>
    /// Пытается снести здание через UI-кнопку.
    /// Если здание защищено от ручного сноса, ничего не делает.
    /// </summary>
    public bool TryDemolishByButton()
    {
        if (!_canBeDemolishedByButton)
        {
            Debug.LogWarning($"{name}: это здание нельзя снести кнопкой.", this);
            return false;
        }

        Demolish();
        return true;
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
        int maxHealth = config.MaxHealth;

        if (factionMember != null && factionMember.Faction == FactionType.Player)
        {
            maxHealth = Mathf.RoundToInt(_techTreeBonusService.ApplyPercentBonus(
                maxHealth,
                TechTreeBonusType.BuildingHp));
        }

        health.Initialize(maxHealth);
    }

    private void PlayDemolishSound()
    {
        _audioService.PlayWorldSound(SoundId.BuildingDemolished, transform.position);
    }

    protected void PlayWorldSound(SoundId soundId, Vector3 position)
    {
        _audioService.PlayWorldSound(soundId, position);
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