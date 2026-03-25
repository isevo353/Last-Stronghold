using UnityEngine;

/// <summary>
/// Снаряд замедления — при попадании замедляет врага на заданное время.
/// </summary>
public class SlowProjectile : MonoBehaviour
{
    public float speed = 7f;
    [Tooltip("Длительность замедления в секундах")]
    public float slowDuration = 3f;
    [Tooltip("Скорость после попадания (0.4 = 40% от обычной)")]
    [Range(0.1f, 1f)]
    public float slowPercent = 0.4f;

    Transform _target;

    public void Init(Transform target)
    {
        _target = target;
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
        TestEnemy enemy = _target.GetComponent<TestEnemy>();
        if (enemy != null)
        {
            enemy.ApplySlow(slowDuration, slowPercent);
        }
        Destroy(gameObject);
    }
}
