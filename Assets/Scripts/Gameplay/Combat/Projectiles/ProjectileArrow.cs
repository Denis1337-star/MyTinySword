using UnityEngine;


/// <summary>
/// Простой проектайл стрелы.
/// Летит к цели и при попадании наносит урон.
/// </summary>
public class ProjectileArrow : MonoBehaviour
{
    private IDamageable target;
    private int damage;
    private float speed;
    private bool initialized;

    public void Initialize(IDamageable target, int damage, float speed)
    {
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

        MonoBehaviour targetBehaviour = target as MonoBehaviour;
        if (targetBehaviour == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = targetBehaviour.transform.position;
        Vector3 direction = targetPosition - transform.position;
        float step = speed * Time.deltaTime;

        if (direction.magnitude <= step)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        transform.position += direction.normalized * step;
    }
}
