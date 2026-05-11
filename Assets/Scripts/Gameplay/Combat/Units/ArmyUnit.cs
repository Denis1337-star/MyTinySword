using UnityEngine;
using Zenject;

/// <summary>
/// Базовый компонент боевого юнита
/// </summary>
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(FactionMember))]
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(UnitAnimatorBridge))]
[RequireComponent(typeof(ArmyUnitBrain))]
[RequireComponent(typeof(Collider2D))]
public sealed class ArmyUnit : ValidatedMonoBehaviour
{
    [SerializeField] private UnitConfig _config;
    [SerializeField] private Health _health;
    [SerializeField] private FactionMember _factionMember;
    [SerializeField] private UnitMovement _movement;
    [SerializeField] private UnitAnimatorBridge _animatorBridge;
    [SerializeField] private ArmyUnitBrain _brain;
    [SerializeField] private Collider2D _bodyCollider;

    private ArmyUnitRegistry _armyUnitRegistry;

    public UnitConfig Config => _config;
    public Health Health => _health;
    public FactionMember FactionMember => _factionMember;
    public UnitMovement Movement => _movement;
    public UnitAnimatorBridge AnimatorBridge => _animatorBridge;
    public ArmyUnitBrain Brain => _brain;
    public Collider2D BodyCollider => _bodyCollider;

    public bool IsDead => _health == null || _health.IsDead;

    [Inject]
    private void Construct(ArmyUnitRegistry armyUnitRegistry)
    {
        _armyUnitRegistry = armyUnitRegistry;
    }

    protected override void Awake()
    {
        ResolveReferences();

        base.Awake();

        if (!enabled)
            return;

        ApplyConfig();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDied += HandleDeath;
    }

    private void Start()
    {
        _armyUnitRegistry?.Register(this);
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= HandleDeath;
    }

    private void OnDestroy()
    {
        _armyUnitRegistry?.Unregister(this);
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _health, nameof(_health));
        valid &= ValidationUtility.IsAssigned(this, _factionMember, nameof(_factionMember));
        valid &= ValidationUtility.IsAssigned(this, _movement, nameof(_movement));
        valid &= ValidationUtility.IsAssigned(this, _animatorBridge, nameof(_animatorBridge));
        valid &= ValidationUtility.IsAssigned(this, _brain, nameof(_brain));
        valid &= ValidationUtility.IsAssigned(this, _bodyCollider, nameof(_bodyCollider));

        if (_config != null && !_config.IsValid())
        {
            Debug.LogError($"{name}: UnitConfig настроен некорректно.", this);
            valid = false;
        }

        return valid;
    }

    public bool IsPlayerUnit()
    {
        return _factionMember != null && _factionMember.IsPlayer();
    }

    public bool IsEnemyUnit()
    {
        return _factionMember != null && _factionMember.IsEnemy();
    }

    private void ApplyConfig()
    {
        _health.Initialize(_config.MaxHealth);
        _movement.SetSpeed(_config.MoveSpeed);
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    private void ResolveReferences()
    {
        if (_health == null)
            _health = GetComponent<Health>();

        if (_factionMember == null)
            _factionMember = GetComponent<FactionMember>();

        if (_movement == null)
            _movement = GetComponent<UnitMovement>();

        if (_animatorBridge == null)
            _animatorBridge = GetComponent<UnitAnimatorBridge>();

        if (_brain == null)
            _brain = GetComponent<ArmyUnitBrain>();

        if (_bodyCollider == null)
            _bodyCollider = GetComponent<Collider2D>();
    }
}