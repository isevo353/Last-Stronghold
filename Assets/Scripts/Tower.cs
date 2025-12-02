using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Targeting")]
    public float range = 3f;
    public float fireCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    float _lastShotTime;

    void Update()
    {
        TestEnemy target = FindTarget();
        if (target == null) return;



        if (Time.time >= _lastShotTime + fireCooldown)
        {
            Shoot(target);
            _lastShotTime = Time.time;
        }
    }

    TestEnemy FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (hits.Length == 0) return null;

        // ближайший враг
        float bestDist = Mathf.Infinity;
        TestEnemy best = null;

        foreach (var h in hits)
        {
            TestEnemy e = h.GetComponent<TestEnemy>();
            if (e == null) continue;

            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = e;
            }
        }
        return best;
    }

    void Shoot(TestEnemy target)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject go = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(target.transform);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}