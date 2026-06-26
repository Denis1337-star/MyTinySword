using UnityEngine;
using Zenject;

/// <summary>
/// Управляет поведением боевого юнита
/// </summary>
[RequireComponent(typeof(ArmyUnit))]
public sealed class ArmyUnitBrain : ValidatedMonoBehaviour
{
    [SerializeField] private ArmyUnit _unit;
    private ArmyTargetFinder _targetFinder;
    private ArmyUnitCombat _combat;
    private GameAudioService _audioService;

    private BrainState _state;
    private Health _currentTarget;
    private Health _currentHealTarget;

    private Vector2 _commandedMoveTarget;
    private bool _hasCommandedMoveTarget;
    private bool _returnToMoveAfterCombat;

    private float _attackTimer;
    private float _healTimer;

    private enum BrainState
    {
        Idle,
        Move,
        Attack,
        Heal
    }

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _unit, nameof(_unit));

        return valid;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        _targetFinder = new ArmyTargetFinder(_unit, transform);
    }
    private void Start()
    {
        _combat = new ArmyUnitCombat(_unit,
            transform, _audioService);
    }

    private void Update()
    {
        if (!CanAct())
            return;

        switch (_state)
        {
            case BrainState.Idle:
                UpdateIdle();
                break;

            case BrainState.Move:
                UpdateMove();
                break;

            case BrainState.Attack:
                UpdateAttack();
                break;

            case BrainState.Heal:
                UpdateHeal();
                break;
        }
    }

    public void MoveTo(Vector2 position)
    {
        if (!CanAct())
            return;

        _currentTarget = null;
        _currentHealTarget = null;

        _commandedMoveTarget = position;
        _hasCommandedMoveTarget = true;
        _returnToMoveAfterCombat = false;

        _state = BrainState.Move;
        _unit.Movement.MoveTo(position);
    }

    public void Attack(IDamageable target)
    {
        if (!CanAct())
            return;

        Health targetHealth = GetHealthFromDamageable(target);

        if (targetHealth == null || targetHealth.IsDead)
            return;

        if (!_targetFinder.IsEnemy(targetHealth))
            return;

        StartAttack(targetHealth, false);
    }

    public void Heal(Health target)
    {
        if (!CanAct())
            return;

        if (_unit.Config.UnitType != ArmyUnitType.Healer)
            return;

        if (target == null || target.IsDead)
            return;

        if (_targetFinder.IsEnemy(target))
            return;

        StartHeal(target, false);
    }

    public void Stop()
    {
        if (!CanAct())
            return;

        _currentTarget = null;
        _currentHealTarget = null;

        _hasCommandedMoveTarget = false;
        _returnToMoveAfterCombat = false;

        _unit.Movement.Stop();
        _state = BrainState.Idle;
    }

    private void UpdateIdle()
    {
        if (_unit.Config.UnitType == ArmyUnitType.Healer)
        {
            Health ally = _targetFinder.FindLowestHealthAllyUnit();

            if (ally != null)
            {
                StartHeal(ally, false);
                return;
            }
        }

        Health enemy = _targetFinder.FindNearestEnemyTarget();

        if (enemy != null)
            StartAttack(enemy, false);
    }

    private void UpdateMove()
    {
        if (_unit.Movement.HasTarget)
            return;

        _hasCommandedMoveTarget = false;
        _returnToMoveAfterCombat = false;

        _state = BrainState.Idle;
    }

    private void UpdateAttack()
    {
        if (_currentTarget == null || _currentTarget.IsDead)
        {
            FinishCombatAction();
            return;
        }

        float distanceToTarget = _combat.GetDistanceToTarget(_currentTarget);

        if (distanceToTarget > _unit.GetAttackRange())
        {
            _unit.Movement.MoveTo(_currentTarget.transform.position);
            return;
        }

        _unit.Movement.Stop();

        _attackTimer -= Time.deltaTime;

        if (_attackTimer > 0f)
            return;

        _combat.PerformAttack(_currentTarget);
        _attackTimer = _unit.GetAttackCooldown();
    }

    private void UpdateHeal()
    {
        if (_unit.Config.UnitType != ArmyUnitType.Healer)
        {
            FinishCombatAction();
            return;
        }

        if (_currentHealTarget == null || _currentHealTarget.IsDead)
        {
            FinishCombatAction();
            return;
        }

        float healRange = _unit.GetHealRange();
        float healRangeSqr = healRange * healRange;
        float distanceSqr = (_currentHealTarget.transform.position - transform.position).sqrMagnitude;

        if (distanceSqr > healRangeSqr)
        {
            _unit.Movement.MoveTo(_currentHealTarget.transform.position);
            return;
        }

        _unit.Movement.Stop();

        _healTimer -= Time.deltaTime;

        if (_healTimer > 0f)
            return;

        _combat.PerformHeal(_currentHealTarget);
        _healTimer = _unit.GetHealCooldown();
    }

    private void StartAttack(Health target, bool returnToMoveAfterCombat)
    {
        _currentTarget = target;
        _currentHealTarget = null;

        _returnToMoveAfterCombat = returnToMoveAfterCombat && _hasCommandedMoveTarget;
        _state = BrainState.Attack;
    }

    private void StartHeal(Health target, bool returnToMoveAfterCombat)
    {
        _currentHealTarget = target;
        _currentTarget = null;

        _returnToMoveAfterCombat = returnToMoveAfterCombat && _hasCommandedMoveTarget;
        _state = BrainState.Heal;
    }

    private void FinishCombatAction()
    {
        _currentTarget = null;
        _currentHealTarget = null;

        if (_returnToMoveAfterCombat && _hasCommandedMoveTarget)
        {
            _returnToMoveAfterCombat = false;
            _state = BrainState.Move;
            _unit.Movement.MoveTo(_commandedMoveTarget);
            return;
        }

        _returnToMoveAfterCombat = false;
        _hasCommandedMoveTarget = false;

        _unit.Movement.Stop();
        _state = BrainState.Idle;
    }

    private Health GetHealthFromDamageable(IDamageable damageable)
    {
        Component component = damageable as Component;

        if (component == null)
            return null;

        return component.GetComponent<Health>();
    }

    private bool CanAct()
    {
        return !_unit.IsDead;
    }
}