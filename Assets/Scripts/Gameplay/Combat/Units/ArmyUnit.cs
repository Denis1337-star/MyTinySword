using UnityEngine;
using Zenject;

/// <summary>
/// Базовый компонент боевого юнита.
/// Работает и для player, и для enemy.
/// </summary>
public class ArmyUnit : MonoBehaviour
{
    [SerializeField] private UnitConfig _config;

    [Header("Runtime Modules")]
    [SerializeField] private Health _health;
    [SerializeField] private FactionMember _factionMember;
    [SerializeField] private UnitMovement _movement;
    [SerializeField] private UnitAnimatorBridge _animatorBridge;
    [SerializeField] private ArmyUnitBrain _brain;

    private ArmyUnitRegistry _armyUnitRegistry;

    public UnitConfig Config => _config;
    public Health Health => _health;
    public FactionMember FactionMember => _factionMember;
    public UnitMovement Movement => _movement;
    public UnitAnimatorBridge AnimatorBridge => _animatorBridge;
    public ArmyUnitBrain Brain => _brain;

    public bool IsDead => _health == null || _health.IsDead;

    [Inject]
    private void Construct(ArmyUnitRegistry armyUnitRegistry)
    {
        _armyUnitRegistry = armyUnitRegistry;
    }

    private void Awake()
    {
        ResolveReferences();
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
        if (_config == null)
            return;

        if (_health != null)
            _health.Initialize(_config.MaxHealth);

        if (_movement != null)
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

        if (_health == null)
            _health = GetComponentInChildren<Health>();

        if (_factionMember == null)
            _factionMember = GetComponent<FactionMember>();

        if (_factionMember == null)
            _factionMember = GetComponentInParent<FactionMember>();

        if (_factionMember == null)
            _factionMember = GetComponentInChildren<FactionMember>();

        if (_movement == null)
            _movement = GetComponent<UnitMovement>();

        if (_movement == null)
            _movement = GetComponentInChildren<UnitMovement>();

        if (_animatorBridge == null)
            _animatorBridge = GetComponent<UnitAnimatorBridge>();

        if (_animatorBridge == null)
            _animatorBridge = GetComponentInChildren<UnitAnimatorBridge>();

        if (_brain == null)
            _brain = GetComponent<ArmyUnitBrain>();

        if (_brain == null)
            _brain = GetComponentInChildren<ArmyUnitBrain>();
    }
}