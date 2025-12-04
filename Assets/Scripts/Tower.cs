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
        // Если shootPoint не назначен в инспекторе - ищем автоматически
        if (shootPoint == null)
        {
            shootPoint = transform.Find("shootPoint");

            if (shootPoint == null)
            {
                Debug.LogError("shootPoint не найден! Создай дочерний объект 'ShootPoint' под Tower");
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
        // Сначала находим всех врагов по компоненту
        TestEnemy[] allEnemies = FindObjectsOfType<TestEnemy>();

        float bestDist = Mathf.Infinity;
        TestEnemy best = null;

        foreach (TestEnemy enemy in allEnemies)
        {
            // Проверяем расстояние до центра врага
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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}