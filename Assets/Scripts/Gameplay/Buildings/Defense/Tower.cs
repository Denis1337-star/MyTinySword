using UnityEngine;

/// <summary>
/// Автоматически ищет врагов в радиусе и атакует их
/// </summary>
public sealed class Tower : BuildingBase
{
    private const int TargetBufferSize = 32;

    [Header("Tower Combat")]
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private int _damage;
    [SerializeField] private ProjectileArrow _arrowPrefab;
    [SerializeField] private float _arrowSpeed;
    [SerializeField] private Transform _shootPoint;

    private readonly Collider2D[] _targetBuffer = new Collider2D[TargetBufferSize];

    private Health _currentTarget;
    private float _attackTimer;

    protected override void OnValidate()
    {
        base.OnValidate();

        _attackRange = Mathf.Max(0.1f, _attackRange);
        _attackCooldown = Mathf.Max(0.1f, _attackCooldown);
        _damage = Mathf.Max(0, _damage);
        _arrowSpeed = Mathf.Max(0.1f, _arrowSpeed);
    }

    private void Update()
    {
        if (Health != null && Health.IsDead)
            return;

        UpdateCombat();
    }

    private void UpdateCombat()
    {
        if (_currentTarget == null || _currentTarget.IsDead || !IsTargetInRange(_currentTarget))
            _currentTarget = FindBestTarget();

        if (_currentTarget == null)
            return;

        _attackTimer -= Time.deltaTime;

        if (_attackTimer > 0f)
            return;

        Shoot(_currentTarget);
        _attackTimer = _attackCooldown;
    }

    private bool IsTargetInRange(Health target)
    {
        if (target == null || target.IsDead)
            return false;

        float distanceSqr = (target.transform.position - transform.position).sqrMagnitude;
        return distanceSqr <= _attackRange * _attackRange;
    }

    private Health FindBestTarget()
    {
        if (FactionMember == null)
            return null;

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            _attackRange,
            _targetBuffer);

        if (hitCount == 0)
            return null;

        Health bestTarget = null;
        int bestPriority = int.MinValue;
        float bestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _targetBuffer[i];

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

        Vector3 spawnPosition = _shootPoint != null ? _shootPoint.position : transform.position;

        if (_arrowPrefab != null)
        {
            ProjectileArrow arrow = Instantiate(_arrowPrefab, spawnPosition, Quaternion.identity);
            arrow.Initialize(target, _damage, _arrowSpeed);
        }
        else
        {
            target.TakeDamage(_damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}