using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemiesCount = 5;
    public float spawnInterval = 1f;

    private Coroutine _spawnCoroutine;
    private bool _isSpawning = false;

    void Start()
    {
        // Ничего не спавним в Start - ждём клика на кнопку
    }

    /// <summary>
    /// Вызывается при клике на кнопку "Начать волну"
    /// </summary>
    public void StartSpawning()
    {
        if (_isSpawning)
        {
            Debug.Log("[EnemySpawner] Спавн уже идёт!");
            return;
        }

        _isSpawning = true;
        _spawnCoroutine = StartCoroutine(SpawnEnemies());
        Debug.Log("[EnemySpawner] Спавн врагов начался!");
    }

    /// <summary>
    /// Останавливает спавн врагов
    /// </summary>
    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }
        _isSpawning = false;
        Debug.Log("[EnemySpawner] Спавн врагов остановлен!");
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            // Инициализируем компонент у каждого врага
            TestEnemy enemyScript = newEnemy.GetComponent<TestEnemy>();
            if (enemyScript != null)
            {
                enemyScript.startDelay = i * spawnInterval;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        _isSpawning = false;
        Debug.Log("[EnemySpawner] Волна закончена!");
    }
}
