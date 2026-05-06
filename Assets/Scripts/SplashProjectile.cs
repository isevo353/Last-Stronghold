using UnityEngine;

/// <summary>
/// Снаряд со сплэш-уроном — при попадании наносит урон всем врагам в радиусе.
/// </summary>
public class SplashProjectile : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 20;
    [Tooltip("Радиус сплэш-урона")]
    public float splashRadius = 1.5f;

    Transform _target;

    public void Init(Transform target, int damageOverride = -1)
    {
        _target = target;
        if (damageOverride >= 0)
            damage = damageOverride;
    }

    void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _target.position) < 0.15f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        Vector2 hitPoint = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint, splashRadius);

        foreach (var col in hits)
        {
            TestEnemy enemy = col.GetComponent<TestEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}
