using UnityEngine;

/// <summary>
/// Ядовитый снаряд: при попадании создает область, которая массово останавливает врагов на время.
/// </summary>
public class PoisonStopProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 5f;

    [Header("Poison Stop")]
    [Tooltip("Радиус массовой остановки")]
    public float effectRadius = 2f;
    [Tooltip("Длительность полной остановки в секундах")]
    public float stopDuration = 1.75f;

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
            Explode();
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, effectRadius);
        foreach (var hit in hits)
        {
            TestEnemy enemy = hit.GetComponent<TestEnemy>();
            if (enemy != null)
            {
                // 0f -> полная остановка; после stopDuration скорость вернется.
                enemy.ApplySlow(stopDuration, 0f);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 0.35f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
