using UnityEngine;
[RequireComponent(typeof(TestEnemy))]
public class SaboteurSlime : MonoBehaviour
{
    [Header("Sabotage")]
    public float sabotageRange = 0.8f;
    public LayerMask towerLayer = -1;
    public int health = 140;
    private TestEnemy _enemy;
    private bool _usedSabotage;
    void Awake()
    {
        _enemy = GetComponent<TestEnemy>();
        if (_enemy != null)
        {
            _enemy.maxHealth = health;
            _enemy.currentHealth = health;
            _enemy.RefreshHealthBar();
        }
    }
    void Update()
    {
        if (_usedSabotage) return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sabotageRange, towerLayer);
        if (colliders == null || colliders.Length == 0) return;
        for (int i = 0; i < colliders.Length; i++)
        {
            Tower tower = colliders[i].GetComponent<Tower>();
            if (tower == null) continue;
            tower.DowngradeOrDestroyByEnemy();
            _usedSabotage = true;
            Destroy(gameObject);
            return;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, sabotageRange);
    }
}