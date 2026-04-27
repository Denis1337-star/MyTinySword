using UnityEngine;


/// <summary>
/// Летит к цели и при попадании наносит урон
/// </summary>
public class ProjectileArrow : MonoBehaviour
{
    private Health target;
    private int damage;
    private float speed;
    private bool initialized;

    public void Initialize(Health target, int damage, float speed)
    {
        if (target == null || damage <= 0 || speed <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        this.target = target;
        this.damage = damage;
        this.speed = speed;

        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        if (target == null || target.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 direction = targetPosition - transform.position;

        float step = speed * Time.deltaTime;

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
        if (target != null && !target.IsDead)
            target.TakeDamage(damage);

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
