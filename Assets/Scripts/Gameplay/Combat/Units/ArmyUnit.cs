using UnityEngine;
using Zenject;

/// <summary>
/// Базовый компонент боевого юнита
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

    private void Start()
    {
        _armyUnitRegistry?.Register(this);
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
    }

    private void ApplyConfig()
    {
        if (_config == null)
            return;

        if (_health != null)
            _health.ResetHealth();
    }
}
