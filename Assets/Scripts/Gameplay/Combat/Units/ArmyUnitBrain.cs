using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Управляет поведением боевого юнита
/// </summary>
public class ArmyUnitBrain : MonoBehaviour
{
    [Header("Components")]
    [FormerlySerializedAs("unit")]
    [SerializeField] private ArmyUnit _unit;

    [FormerlySerializedAs("movement")]
    [SerializeField] private UnitMovement _movement;

    [FormerlySerializedAs("health")]
    [SerializeField] private Health _health;

    [FormerlySerializedAs("factionMember")]
    [SerializeField] private FactionMember _factionMember;

    [FormerlySerializedAs("animatorBridge")]
    [SerializeField] private UnitAnimatorBridge _animatorBridge;

    private IDamageable _currentTarget;
    private Health _currentHealTarget;

    private Vector3 _commandedMoveTarget;
    private float _actionTimer;

    private bool _hasCommandedMoveTarget;
    private bool _returnToMoveAfterCombat;

    private BrainState _currentState = BrainState.Idle;

    private UnitConfig Config => _unit != null ? _unit.Config : null;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (_health != null && _health.IsDead)
            return;

        switch (_currentState)
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

    /// <summary>
    /// Даёт юниту команду двигаться в указанную позицию.
    /// </summary>
    public void MoveTo(Vector3 position)
    {
        _currentTarget = null;
        _currentHealTarget = null;

        _commandedMoveTarget = position;
        _hasCommandedMoveTarget = true;
        _returnToMoveAfterCombat = false;

        _movement?.MoveTo(position);
        _currentState = BrainState.Move;
    }

    /// <summary>
    /// Даёт юниту команду атаковать конкретную цель.
    /// </summary>
    public void Attack(IDamageable target)
    {
        if (target == null)
            return;

        _currentTarget = target;
        _currentHealTarget = null;

        _hasCommandedMoveTarget = false;
        _returnToMoveAfterCombat = false;

        _currentState = BrainState.Attack;
    }

    private void UpdateIdle()
    {
        if (Config == null)
            return;

        if (Config.UnitType == ArmyUnitType.Healer)
        {
            TryAutoAcquireHealTarget(returnToMoveAfterFind: false);
            return;
        }

        TryAutoAcquireEnemyTarget(returnToMoveAfterFind: false);
    }

    private void UpdateMove()
    {
        if (Config == null)
            return;

        bool foundActionTarget = Config.UnitType == ArmyUnitType.Healer
            ? TryAutoAcquireHealTarget(returnToMoveAfterFind: true)
            : TryAutoAcquireEnemyTarget(returnToMoveAfterFind: true);

        if (foundActionTarget)
            return;

        if (_movement == null)
            return;

        if (!_movement.IsMoving)
            _currentState = BrainState.Idle;
    }

    private void UpdateAttack()
    {
        if (Config == null)
            return;

        if (_currentTarget == null || _currentTarget.IsDead)
        {
            OnActionFinished();
            return;
        }

        Transform targetTransform = (_currentTarget as MonoBehaviour)?.transform;
        if (targetTransform == null)
        {
            OnActionFinished();
            return;
        }

        float distance = Vector3.Distance(transform.position, targetTransform.position);

        if (distance > Config.AttackRange)
        {
            _movement?.MoveTo(targetTransform.position);
            return;
        }

        _movement?.Stop();

        _actionTimer -= Time.deltaTime;
        if (_actionTimer > 0f)
            return;

        _animatorBridge?.PlayAttack();

        if (Config.UnitType == ArmyUnitType.Archer)
            ShootArrow(_currentTarget);
        else
            DealMeleeDamage(_currentTarget);

        _actionTimer = Config.AttackCooldown;
    }

    private void UpdateHeal()
    {
        if (Config == null)
            return;

        if (_currentHealTarget == null ||
            _currentHealTarget.IsDead ||
            _currentHealTarget.CurrentHealth >= _currentHealTarget.MaxHealth)
        {
            OnActionFinished();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            _currentHealTarget.transform.position);

        if (distance > Config.HealRange)
        {
            _movement?.MoveTo(_currentHealTarget.transform.position);
            return;
        }

        _movement?.Stop();

        _actionTimer -= Time.deltaTime;
        if (_actionTimer > 0f)
            return;

        _animatorBridge?.PlayAttack();
        _currentHealTarget.Heal(Config.HealAmount);

        _actionTimer = Config.HealCooldown;
    }

    private bool TryAutoAcquireEnemyTarget(bool returnToMoveAfterFind)
    {
        IDamageable target = FindNearestEnemyTarget();
        if (target == null)
            return false;

        _currentTarget = target;
        _currentHealTarget = null;

        _returnToMoveAfterCombat = returnToMoveAfterFind;
        _currentState = BrainState.Attack;

        return true;
    }

    private bool TryAutoAcquireHealTarget(bool returnToMoveAfterFind)
    {
        Health target = FindLowestHealthAllyUnit();
        if (target == null)
            return false;

        _currentHealTarget = target;
        _currentTarget = null;

        _returnToMoveAfterCombat = returnToMoveAfterFind;
        _currentState = BrainState.Heal;

        return true;
    }

    private IDamageable FindNearestEnemyTarget()
    {
        if (_factionMember == null || Config == null)
            return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            Config.VisionRange);

        if (hits == null || hits.Length == 0)
            return null;

        IDamageable bestTarget = null;
        int bestPriority = int.MinValue;
        float bestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = TryGetDamageableTarget(hit);
            if (damageable == null)
                continue;

            MonoBehaviour targetBehaviour = damageable as MonoBehaviour;
            if (targetBehaviour == null || targetBehaviour.gameObject == gameObject)
                continue;

            FactionMember targetFaction = GetFactionMember(targetBehaviour);
            if (targetFaction == null || !_factionMember.IsEnemy(targetFaction))
                continue;

            int priority = GetTargetPriority(targetBehaviour);
            float distance = Vector3.Distance(
                transform.position,
                targetBehaviour.transform.position);

            bool betterPriority = priority > bestPriority;
            bool samePriorityButCloser = priority == bestPriority && distance < bestDistance;

            if (!betterPriority && !samePriorityButCloser)
                continue;

            bestPriority = priority;
            bestDistance = distance;
            bestTarget = damageable;
        }

        return bestTarget;
    }

    private Health FindLowestHealthAllyUnit()
    {
        if (_factionMember == null || Config == null)
            return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            Config.VisionRange);

        if (hits == null || hits.Length == 0)
            return null;

        Health bestTarget = null;
        float lowestHealthPercent = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Health targetHealth = TryGetHealth(hit);
            if (targetHealth == null || targetHealth.IsDead)
                continue;

            ArmyUnit allyArmyUnit = hit.GetComponent<ArmyUnit>();
            if (allyArmyUnit == null)
                allyArmyUnit = hit.GetComponentInParent<ArmyUnit>();

            if (allyArmyUnit == null)
                continue;

            FactionMember allyFaction = allyArmyUnit.FactionMember;
            if (allyFaction == null || !_factionMember.IsAlly(allyFaction))
                continue;

            if (targetHealth.CurrentHealth >= targetHealth.MaxHealth)
                continue;

            float healthPercent = (float)targetHealth.CurrentHealth / targetHealth.MaxHealth;
            if (healthPercent >= lowestHealthPercent)
                continue;

            lowestHealthPercent = healthPercent;
            bestTarget = targetHealth;
        }

        return bestTarget;
    }

    private void DealMeleeDamage(IDamageable target)
    {
        if (Config == null || target == null)
            return;

        target.TakeDamage(Config.Damage);
    }

    private void ShootArrow(IDamageable target)
    {
        if (Config == null || target == null || Config.ArrowPrefab == null)
        {
            DealMeleeDamage(target);
            return;
        }

        Health targetHealth = target as Health;
        if (targetHealth == null)
        {
            DealMeleeDamage(target);
            return;
        }

        ProjectileArrow arrow = Instantiate(
            Config.ArrowPrefab,
            transform.position,
            Quaternion.identity);

        arrow.Initialize(targetHealth, Config.Damage, Config.ArrowSpeed);
    }

    private void OnActionFinished()
    {
        _currentTarget = null;
        _currentHealTarget = null;

        if (_returnToMoveAfterCombat && _hasCommandedMoveTarget)
        {
            _returnToMoveAfterCombat = false;
            _movement?.MoveTo(_commandedMoveTarget);
            _currentState = BrainState.Move;
            return;
        }

        _currentState = BrainState.Idle;
    }

    private void ResolveReferences()
    {
        if (_unit == null)
            _unit = GetComponent<ArmyUnit>();

        if (_movement == null)
            _movement = GetComponent<UnitMovement>();

        if (_health == null)
            _health = GetComponent<Health>();

        if (_factionMember == null)
            _factionMember = GetComponent<FactionMember>();

        if (_animatorBridge == null)
            _animatorBridge = GetComponent<UnitAnimatorBridge>();
    }

    private static IDamageable TryGetDamageableTarget(Collider2D hit)
    {
        if (hit == null)
            return null;

        IDamageable damageable = hit.GetComponent<IDamageable>();
        if (damageable == null)
            damageable = hit.GetComponentInParent<IDamageable>();

        if (damageable == null || damageable.IsDead)
            return null;

        return damageable;
    }

    private static Health TryGetHealth(Collider2D hit)
    {
        if (hit == null)
            return null;

        Health health = hit.GetComponent<Health>();
        if (health == null)
            health = hit.GetComponentInParent<Health>();

        return health;
    }

    private static FactionMember GetFactionMember(MonoBehaviour targetBehaviour)
    {
        if (targetBehaviour == null)
            return null;

        FactionMember factionMember = targetBehaviour.GetComponent<FactionMember>();
        if (factionMember == null)
            factionMember = targetBehaviour.GetComponentInParent<FactionMember>();

        return factionMember;
    }

    private static int GetTargetPriority(MonoBehaviour targetBehaviour)
    {
        if (targetBehaviour == null)
            return (int)TargetPriorityType.Building;

        CombatTargetInfo targetInfo = targetBehaviour.GetComponent<CombatTargetInfo>();
        if (targetInfo == null)
            targetInfo = targetBehaviour.GetComponentInParent<CombatTargetInfo>();

        return targetInfo != null
            ? (int)targetInfo.TargetPriority
            : (int)TargetPriorityType.Building;
    }

    private void OnDrawGizmosSelected()
    {
        if (_unit == null || _unit.Config == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _unit.Config.VisionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _unit.Config.AttackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _unit.Config.HealRange);
    }
}
