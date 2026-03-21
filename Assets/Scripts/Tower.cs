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

    void Start()
    {
        // ˜˜˜˜ shootPoint ˜˜ ˜˜˜˜˜˜˜˜ ˜ ˜˜˜˜˜˜˜˜˜˜ - ˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜˜
        if (shootPoint == null)
        {
            shootPoint = transform.Find("shootPoint");

            if (shootPoint == null)
            {
                Debug.LogError("shootPoint ˜˜ ˜˜˜˜˜˜! ˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜ 'ShootPoint' ˜˜˜ Tower");
            }
        }
    }

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
        // ˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜ ˜˜˜˜˜˜ ˜˜ ˜˜˜˜˜˜˜˜˜˜
        TestEnemy[] allEnemies = FindObjectsOfType<TestEnemy>();

        float bestDist = Mathf.Infinity;
        TestEnemy best = null;

        foreach (TestEnemy enemy in allEnemies)
        {
            // ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜
            float d = Vector2.Distance(transform.position, enemy.transform.position);

            if (d <= range && d < bestDist)
            {
                bestDist = d;
                best = enemy;
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
        else
        {
            SplashProjectile splash = go.GetComponent<SplashProjectile>();
            if (splash != null)
                splash.Init(target.transform);
            else
            {
                SlowProjectile slow = go.GetComponent<SlowProjectile>();
                if (slow != null)
                    slow.Init(target.transform);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}