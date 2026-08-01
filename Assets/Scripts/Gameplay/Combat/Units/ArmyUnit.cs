using UnityEngine;
using Zenject;

/// <summary>
/// Базовый компонент боевого юнита
/// </summary>
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(UnitAnimatorBridge))]
[RequireComponent(typeof(Collider2D))]
public sealed class ArmyUnit : ValidatedMonoBehaviour
{
    [SerializeField] private FactionType _faction = FactionType.Player;
    [SerializeField] private UnitConfig _config;
    [SerializeField] private TargetPriorityType _targetPriority = TargetPriorityType.ArmyUnit;
    [SerializeField] private Health _health;
    [SerializeField] private UnitMovement _movement;
    [SerializeField] private UnitAnimatorBridge _animatorBridge;
    [SerializeField] private Collider2D _bodyCollider;

    private ArmyUnitRegistry _armyUnitRegistry;
    private TechTreeBonusService _techTreeBonusService;
    private ArmyUnitBrain _brain;

    public UnitConfig Config => _config;
    public TargetPriorityType TargetPriority => _targetPriority;
    public Health Health => _health;
    public UnitMovement Movement => _movement;
    public UnitAnimatorBridge AnimatorBridge => _animatorBridge;
    public ArmyUnitBrain Brain => _brain;
    public FactionType Faction => _faction;

    public Collider2D BodyCollider => _bodyCollider;

    public bool IsDead => _health.IsDead;

    [Inject]
    private void Construct(
      ArmyUnitRegistry armyUnitRegistry,
      TechTreeBonusService techTreeBonusService,
      GameAudioService audioService)
    {
        _armyUnitRegistry = armyUnitRegistry;
        _techTreeBonusService = techTreeBonusService;
        _brain = new ArmyUnitBrain(this, audioService);
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        ApplyConfig();
    }
    private void Update()
    {
        _brain.Tick();
    }

    private void OnEnable()
    {
            _health.OnDied += HandleDeath;
    }

    private void Start()
    {
        // После UnitMovement.ConfigureAgent: воины проходят сквозь других агентов.
        _movement.SetAvoidOtherAgents(false);
        _movement.SetAvoidancePriority(0);
        _armyUnitRegistry.Register(this);
    }

    private void OnDisable()
    {
            _health.OnDied -= HandleDeath;
    }

    private void OnDestroy()
    {
        _armyUnitRegistry.Unregister(this);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsValidConfig(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _health, nameof(_health));
        valid &= ValidationUtility.IsAssigned(this, _movement, nameof(_movement));
        valid &= ValidationUtility.IsAssigned(this, _animatorBridge, nameof(_animatorBridge));
        valid &= ValidationUtility.IsAssigned(this, _bodyCollider, nameof(_bodyCollider));

        return valid;
    }
    public bool IsPlayerUnit() => FactionRules.IsPlayer(_faction);
    public bool IsEnemyUnit() => FactionRules.IsEnemy(_faction);
    private void ApplyConfig()
    {
        _health.Initialize(GetMaxHealth());
        _movement.SetSpeed(GetMoveSpeed());
    }
    public int GetMaxHealth()
    {
        return Mathf.RoundToInt(ApplyStatsArmyBonus(_config.MaxHealth));
    }

    public float GetMoveSpeed()
    {
        return ApplyStatsArmyBonus(_config.MoveSpeed);
    }

    public int GetDamage()
    {
        return Mathf.RoundToInt(ApplyStatsArmyBonus(_config.Damage));
    }

    public float GetAttackRange()
    {
        return ApplyStatsArmyBonus(_config.AttackRange);
    }

    public float GetAttackCooldown()
    {
        float cooldown = ApplyStatsArmyReduction(_config.AttackCooldown);
        return Mathf.Max(0.1f, cooldown);
    }

    public int GetHealAmount()
    {
        return Mathf.RoundToInt(ApplyStatsArmyBonus(_config.HealAmount));
    }

    public float GetHealRange()
    {
        return ApplyStatsArmyBonus(_config.HealRange);
    }

    public float GetHealCooldown()
    {
        float cooldown = ApplyStatsArmyReduction(_config.HealCooldown);
        return Mathf.Max(0.1f, cooldown);
    }

    public float GetArrowSpeed()
    {
        return ApplyStatsArmyBonus(_config.ArrowSpeed);
    }

    private float ApplyStatsArmyBonus(float baseValue)
    {
        if (!IsPlayerUnit())
            return baseValue;

        return _techTreeBonusService.ApplyPercentBonus(
            baseValue,
            TechTreeBonusType.StatsArmy);
    }

    private float ApplyStatsArmyReduction(float baseValue)
    {
        if (!IsPlayerUnit())
            return baseValue;

        return _techTreeBonusService.ApplyPercentReduction(
            baseValue,
            TechTreeBonusType.StatsArmy);
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}