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
        if (!IsCurrentTargetValid())
            _currentTarget = FindBestTarget();

        if (_currentTarget == null)
            return;

        _attackTimer -= Time.deltaTime;

        if (_attackTimer > 0f)
            return;

        Shoot(_currentTarget);
        _attackTimer = _attackCooldown;
    }
    private bool IsCurrentTargetValid()
    {
        if (_currentTarget == null)
            return false;

        if (_currentTarget.IsDead)
            return false;

        return IsTargetInRange(_currentTarget);
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

            if (!TryGetValidTarget(hit, out Health targetHealth, out CombatTargetInfo targetInfo))
                continue;

            int priority = GetTargetPriority(targetInfo);
            float distanceSqr = GetDistanceSqrTo(targetHealth);

            if (!IsBetterTarget(priority, distanceSqr, bestPriority, bestDistanceSqr))
                continue;

            bestPriority = priority;
            bestDistanceSqr = distanceSqr;
            bestTarget = targetHealth;
        }

        return bestTarget;
    }

    private bool TryGetValidTarget(
         Collider2D hit,
         out Health targetHealth,
         out CombatTargetInfo targetInfo)
    {
        targetHealth = null;
        targetInfo = null;

        if (hit == null)
            return false;

        if (!hit.TryGetComponent(out targetHealth))
            return false;

        if (targetHealth.IsDead)
            return false;

        if (targetHealth == Health)
            return false;

        if (!hit.TryGetComponent(out FactionMember targetFaction))
            return false;

        if (!FactionMember.IsEnemy(targetFaction))
            return false;

        hit.TryGetComponent(out targetInfo);
        return true;
    }

    private int GetTargetPriority(CombatTargetInfo targetInfo)
    {
        if (targetInfo == null)
            return (int)TargetPriorityType.Building;

        return (int)targetInfo.TargetPriority;
    }

    private float GetDistanceSqrTo(Health target)
    {
        return (target.transform.position - transform.position).sqrMagnitude;
    }

    private bool IsBetterTarget(
        int priority,
        float distanceSqr,
        int bestPriority,
        float bestDistanceSqr)
    {
        if (priority > bestPriority)
            return true;

        if (priority < bestPriority)
            return false;

        return distanceSqr < bestDistanceSqr;
    }

    private void Shoot(Health target)
    {
        if (target == null || target.IsDead)
            return;

        Vector3 spawnPosition = _shootPoint != null
            ? _shootPoint.position
            : transform.position;

        PlayAttackSound(spawnPosition);

        if (_arrowPrefab != null)
        {
            ProjectileArrow arrow = Instantiate(_arrowPrefab, spawnPosition, Quaternion.identity);
            arrow.Initialize(target, _damage, _arrowSpeed);
            return;
        }

        target.TakeDamage(_damage);
    }

    private void PlayAttackSound(Vector3 position)
    {
        PlayWorldSound(SoundId.ArrowShoot, position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}