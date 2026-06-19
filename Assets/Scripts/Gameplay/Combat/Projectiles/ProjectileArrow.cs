using UnityEngine;

/// <summary>
/// Летит к цели и при попадании наносит урон
/// </summary>
public sealed class ProjectileArrow : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _maxLifetime = 5f;

    private Health _target;
    private int _damage;
    private float _speed;
    private float _lifeTimer;
    private bool _initialized;

    public void Initialize(Health target, int damage, float speed)
    {
        if (target == null || damage <= 0 || speed <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        _target = target;
        _damage = damage;
        _speed = speed;
        _lifeTimer = 0f;

        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized)
            return;

        _lifeTimer += Time.deltaTime;

        if (_lifeTimer >= _maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (_target == null || _target.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector3 targetPosition = _target.transform.position;
        Vector3 direction = targetPosition - transform.position;

        float step = _speed * Time.deltaTime;

        if (direction.sqrMagnitude <= step * step)
        {
            HitTarget();
            return;
        }

        Vector3 moveDirection = direction.normalized;
        transform.position += moveDirection * step;

        RotateToDirection(moveDirection);
    }

    private void HitTarget()
    {
        if (_target != null && !_target.IsDead)
            _target.TakeDamage(_damage);

        Destroy(gameObject);
    }

    private void RotateToDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}