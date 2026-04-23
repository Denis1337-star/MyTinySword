using UnityEngine;

/// <summary>
/// ”правл€ет поведением боевого юнита:
/// движение, атака, цели.
/// </summary>
public class ArmyUnitBrain : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ArmyUnit unit;
    [SerializeField] private UnitMovement movement;
    [SerializeField] private Health health;
    [SerializeField] private FactionMember factionMember;
    [SerializeField] private UnitAnimatorBridge animatorBridge;

    private IDamageable currentTarget;
    private Health currentHealTarget;
    private float actionTimer;

    private Vector3 commandedMoveTarget;
    private bool hasCommandedMoveTarget;
    private bool returnToMoveAfterCombat;

    private enum State
    {
        Idle,
        Move,
        Attack,
        Heal
    }

    private State currentState = State.Idle;

    private UnitConfig Config => unit != null ? unit.Config : null;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<ArmyUnit>();

        if (movement == null)
            movement = GetComponent<UnitMovement>();

        if (health == null)
            health = GetComponent<Health>();

        if (factionMember == null)
            factionMember = GetComponent<FactionMember>();

        if (animatorBridge == null)
            animatorBridge = GetComponent<UnitAnimatorBridge>();
    }

    private void Update()
    {
        if (health != null && health.IsDead)
            return;

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Move:
                UpdateMove();
                break;

            case State.Attack:
                UpdateAttack();
                break;

            case State.Heal:
                UpdateHeal();
                break;
        }
    }

    public void MoveTo(Vector3 position)
    {
        currentTarget = null;
        currentHealTarget = null;

        commandedMoveTarget = position;
        hasCommandedMoveTarget = true;
        returnToMoveAfterCombat = false;

        movement?.MoveTo(position);
        currentState = State.Move;
    }

    public void Attack(IDamageable target)
    {
        if (target == null)
            return;

        currentTarget = target;
        currentHealTarget = null;
        hasCommandedMoveTarget = false;
        returnToMoveAfterCombat = false;

        currentState = State.Attack;
    }

    private void UpdateIdle()
    {
        if (Config == null)
            return;

        if (Config.UnitType == ArmyUnitType.Healer)
        {
            if (TryAutoAcquireHealTarget(returnToMoveAfterFind: false))
                return;
        }
        else
        {
            if (TryAutoAcquireEnemyTarget(returnToMoveAfterFind: false))
                return;
        }
    }

    private void UpdateMove()
    {
        if (Config == null)
            return;

        if (Config.UnitType == ArmyUnitType.Healer)
        {
            if (TryAutoAcquireHealTarget(returnToMoveAfterFind: true))
                return;
        }
        else
        {
            if (TryAutoAcquireEnemyTarget(returnToMoveAfterFind: true))
                return;
        }

        if (movement == null)
            return;

        if (!movement.IsMoving)
            currentState = State.Idle;
    }

    private void UpdateAttack()
    {
        if (Config == null)
            return;

        if (currentTarget == null || currentTarget.IsDead)
        {
            OnActionFinished();
            return;
        }

        Transform targetTransform = (currentTarget as MonoBehaviour)?.transform;
        if (targetTransform == null)
        {
            OnActionFinished();
            return;
        }

        float distance = Vector3.Distance(transform.position, targetTransform.position);

        if (distance > Config.attackRange)
        {
            movement?.MoveTo(targetTransform.position);
            return;
        }

        movement?.Stop();

        actionTimer -= Time.deltaTime;
        if (actionTimer > 0f)
            return;

        animatorBridge?.PlayAttack();

        if (Config.UnitType == ArmyUnitType.Archer)
            ShootArrow(currentTarget);
        else
            DealMeleeDamage(currentTarget);

        actionTimer = Config.attackCooldown;
    }

    private void UpdateHeal()
    {
        if (Config == null)
            return;

        if (currentHealTarget == null || currentHealTarget.IsDead || currentHealTarget.CurrentHealth >= currentHealTarget.MaxHealth)
        {
            OnActionFinished();
            return;
        }

        float distance = Vector3.Distance(transform.position, currentHealTarget.transform.position);

        if (distance > Config.healRange)
        {
            movement?.MoveTo(currentHealTarget.transform.position);
            return;
        }

        movement?.Stop();

        actionTimer -= Time.deltaTime;
        if (actionTimer > 0f)
            return;

        animatorBridge?.PlayAttack();
        currentHealTarget.Heal(Config.healAmount);

        actionTimer = Config.healCooldown;
    }

    private bool TryAutoAcquireEnemyTarget(bool returnToMoveAfterFind)
    {
        IDamageable target = FindNearestEnemyTarget();
        if (target == null)
            return false;

        currentTarget = target;
        currentHealTarget = null;
        returnToMoveAfterCombat = returnToMoveAfterFind;
        currentState = State.Attack;
        return true;
    }

    private bool TryAutoAcquireHealTarget(bool returnToMoveAfterFind)
    {
        Health target = FindLowestHealthAllyUnit();
        if (target == null)
            return false;

        currentHealTarget = target;
        currentTarget = null;
        returnToMoveAfterCombat = returnToMoveAfterFind;
        currentState = State.Heal;
        return true;
    }

    private IDamageable FindNearestEnemyTarget()
    {
        if (factionMember == null || Config == null)
            return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Config.visionRange);
        if (hits == null || hits.Length == 0)
            return null;

        IDamageable bestTarget = null;
        int bestPriority = int.MinValue;
        float bestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable == null)
                damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null || damageable.IsDead)
                continue;

            MonoBehaviour targetBehaviour = damageable as MonoBehaviour;
            if (targetBehaviour == null)
                continue;

            if (targetBehaviour.gameObject == gameObject)
                continue;

            FactionMember targetFaction = targetBehaviour.GetComponent<FactionMember>();
            if (targetFaction == null)
                targetFaction = targetBehaviour.GetComponentInParent<FactionMember>();

            if (targetFaction == null || !factionMember.IsEnemy(targetFaction))
                continue;

            CombatTargetInfo targetInfo = targetBehaviour.GetComponent<CombatTargetInfo>();
            if (targetInfo == null)
                targetInfo = targetBehaviour.GetComponentInParent<CombatTargetInfo>();

            int priority = targetInfo != null
                ? (int)targetInfo.TargetPriority
                : (int)TargetPriorityType.Building;

            float distance = Vector3.Distance(transform.position, targetBehaviour.transform.position);

            bool betterPriority = priority > bestPriority;
            bool samePriorityButCloser = priority == bestPriority && distance < bestDistance;

            if (betterPriority || samePriorityButCloser)
            {
                bestPriority = priority;
                bestDistance = distance;
                bestTarget = damageable;
            }
        }

        return bestTarget;
    }

    private Health FindLowestHealthAllyUnit()
    {
        if (factionMember == null || Config == null)
            return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Config.visionRange);
        if (hits == null || hits.Length == 0)
            return null;

        Health bestTarget = null;
        float lowestHealthPercent = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            Health targetHealth = hit.GetComponent<Health>();
            if (targetHealth == null)
                targetHealth = hit.GetComponentInParent<Health>();

            if (targetHealth == null || targetHealth.IsDead)
                continue;

            ArmyUnit allyArmyUnit = hit.GetComponent<ArmyUnit>();
            if (allyArmyUnit == null)
                allyArmyUnit = hit.GetComponentInParent<ArmyUnit>();

            if (allyArmyUnit == null)
                continue;

            FactionMember allyFaction = allyArmyUnit.FactionMember;
            if (allyFaction == null || !factionMember.IsAlly(allyFaction))
                continue;

            if (targetHealth.CurrentHealth >= targetHealth.MaxHealth)
                continue;

            float healthPercent = (float)targetHealth.CurrentHealth / targetHealth.MaxHealth;
            if (healthPercent < lowestHealthPercent)
            {
                lowestHealthPercent = healthPercent;
                bestTarget = targetHealth;
            }
        }

        return bestTarget;
    }

    private void DealMeleeDamage(IDamageable target)
    {
        if (Config == null || target == null)
            return;

        target.TakeDamage(Config.damage);
    }

    private void ShootArrow(IDamageable target)
    {
        if (Config == null || target == null || Config.ArrowPrefab == null)
        {
            DealMeleeDamage(target);
            return;
        }

        ProjectileArrow arrow = Instantiate(Config.ArrowPrefab, transform.position, Quaternion.identity);
        arrow.Initialize(target, Config.damage, Config.arrowSpeed);
    }

    private void OnActionFinished()
    {
        currentTarget = null;
        currentHealTarget = null;

        if (returnToMoveAfterCombat && hasCommandedMoveTarget)
        {
            returnToMoveAfterCombat = false;
            movement?.MoveTo(commandedMoveTarget);
            currentState = State.Move;
            return;
        }

        currentState = State.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        if (unit == null || unit.Config == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, unit.Config.visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, unit.Config.attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, unit.Config.healRange);
    }
}
