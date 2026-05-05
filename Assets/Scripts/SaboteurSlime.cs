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

        // ДИАГНОСТИКА КАЖДЫЙ КАДР
        Debug.Log($"Слизень на позиции {transform.position}, ищу башни в радиусе {sabotageRange}");
    
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sabotageRange, towerLayer);
    
        Debug.Log($"Найдено коллайдеров: {colliders?.Length ?? 0}");
    
        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log($"Коллайдер {i}: {colliders[i].name}, тег={colliders[i].tag}, слой={LayerMask.LayerToName(colliders[i].gameObject.layer)}");
        
            Tower tower = colliders[i].GetComponent<Tower>();
            if (tower == null)
            {
                Debug.Log($"У {colliders[i].name} нет компонента Tower");
                continue;
             }
        
            Debug.Log($"НАШЁЛ БАШНЮ! Вызываю уничтожение {tower.name}");
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
