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

        return CombatTargetScanner.FindBestTarget(
            transform.position,
            _attackRange,
            _targetBuffer,
            IsValidEnemyTarget,
            GetTargetPriority);
    }

    private bool IsValidEnemyTarget(Collider2D hit, Health targetHealth)
    {
        if (hit == null || targetHealth == null)
            return false;

        if (targetHealth.IsDead)
            return false;

        if (targetHealth == Health)
            return false;

        if (!hit.TryGetComponent(out FactionMember targetFaction))
            return false;

        return FactionMember.IsEnemy(targetFaction);
    }

    private int GetTargetPriority(Collider2D hit)
    {
        if (hit != null && hit.TryGetComponent(out CombatTargetInfo targetInfo))
            return (int)targetInfo.TargetPriority;

        return (int)TargetPriorityType.Building;
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
