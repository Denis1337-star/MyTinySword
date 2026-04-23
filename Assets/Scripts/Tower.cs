using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Оборонительная башня.
/// Автоматически ищет врагов в радиусе и атакует их с расстояния.
/// </summary>
public class Tower : BuildingBase
{
    [Header("Tower Combat")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int damage = 15;
    [SerializeField] private ProjectileArrow arrowPrefab;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private Transform shootPoint;

    private IDamageable currentTarget;
    private float attackTimer;

    private void OnValidate()
    {
        if (shootPoint == null)
        {
            Transform child = transform.Find("ShootPoint");
            if (child != null)
                shootPoint = child;
        }

        attackRange = Mathf.Max(0.1f, attackRange);
        attackCooldown = Mathf.Max(0.1f, attackCooldown);
        damage = Mathf.Max(0, damage);
        arrowSpeed = Mathf.Max(0.1f, arrowSpeed);
    }

    private void Update()
    {
        if (!enabled)
            return;

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

    private bool IsTargetInRange(IDamageable target)
    {
        MonoBehaviour targetBehaviour = target as MonoBehaviour;
        if (targetBehaviour == null)
            return false;

        float distance = Vector3.Distance(transform.position, targetBehaviour.transform.position);
        return distance <= attackRange;
    }

    private IDamageable FindBestTarget()
    {
        if (FactionMember == null)
            return null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
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

            if (targetFaction == null || !FactionMember.IsEnemy(targetFaction))
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

    private void Shoot(IDamageable target)
    {
        if (target == null)
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
