using UnityEngine;

/// <summary>
/// Автоматически ищет врагов в радиусе и атакует их 
/// </summary>
public class Tower : BuildingBase
{
    [Header("Tower Combat")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int damage;
    [SerializeField] private ProjectileArrow arrowPrefab;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private Transform shootPoint;

    private Health currentTarget;
    private float attackTimer;

    private void OnValidate()
    {
        attackRange = Mathf.Max(0.1f, attackRange);
        attackCooldown = Mathf.Max(0.1f, attackCooldown);
        damage = Mathf.Max(0, damage);
        arrowSpeed = Mathf.Max(0.1f, arrowSpeed);
    }

    private void Update()
    {
        if (Health != null && Health.IsDead)
            return;

        UpdateCombat();
    }

    private void UpdateCombat()
    {
        if (currentTarget == null || currentTarget.IsDead || !IsTargetInRange(currentTarget))
            currentTarget = FindBestTarget();

        if (currentTarget == null)
            return;

        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f)
            return;

        Shoot(currentTarget);
        attackTimer = attackCooldown;
    }

    private bool IsTargetInRange(Health target)
    {
        if (target == null || target.IsDead)
            return false;

        float distanceSqr = (target.transform.position - transform.position).sqrMagnitude;
        return distanceSqr <= attackRange * attackRange;
    }

    private Health FindBestTarget()
    {
        if (FactionMember == null)
            return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        if (hits == null || hits.Length == 0)
            return null;

        Health bestTarget = null;
        int bestPriority = int.MinValue;
        float bestDistanceSqr = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            Health targetHealth = hit.GetComponentInParent<Health>();
            if (targetHealth == null || targetHealth.IsDead)
                continue;

            if (targetHealth.gameObject == gameObject)
                continue;

            FactionMember targetFaction = targetHealth.GetComponentInParent<FactionMember>();
            if (targetFaction == null || !FactionMember.IsEnemy(targetFaction))
                continue;

            CombatTargetInfo targetInfo = targetHealth.GetComponentInParent<CombatTargetInfo>();

            int priority = targetInfo != null
                ? (int)targetInfo.TargetPriority
                : (int)TargetPriorityType.Building;

            float distanceSqr = (targetHealth.transform.position - transform.position).sqrMagnitude;

            bool betterPriority = priority > bestPriority;
            bool samePriorityButCloser = priority == bestPriority && distanceSqr < bestDistanceSqr;

            if (betterPriority || samePriorityButCloser)
            {
                bestPriority = priority;
                bestDistanceSqr = distanceSqr;
                bestTarget = targetHealth;
            }
        }

        return bestTarget;
    }

    private void Shoot(Health target)
    {
        if (target == null || target.IsDead)
            return;

        Vector3 spawnPosition = shootPoint != null ? shootPoint.position : transform.position;

        if (arrowPrefab != null)
        {
            ProjectileArrow arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
            arrow.Initialize(target, damage, arrowSpeed);
        }
        else
        {
            target.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
