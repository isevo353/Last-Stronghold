using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Перетащи префаб врага сюда
    public int enemiesCount = 5;
    public float spawnInterval = 1f;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            // Устанавливаем задержку для каждого врага
            TestEnemy enemyScript = newEnemy.GetComponent<TestEnemy>();
            if (enemyScript != null)
            {
                enemyScript.startDelay = i * spawnInterval;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}