using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalEnemyPrefab;     // Обычный орк
    public GameObject armoredEnemyPrefab;    // Бронированный орк

    [Header("Wave Settings")]
    public int waveNumber = 1;               // Текущая волна
    public int baseNormalEnemies = 5;        // Базовое количество обычных орков
    public int baseArmoredEnemies = 1;       // Базовое количество бронированных
    public float spawnInterval = 1f;         // Задержка между спавном

    private Coroutine _spawnCoroutine;
    private bool _isSpawning = false;

    void Start()
    {
        waveNumber = 1;
        Debug.Log($"[EnemySpawner] Готов. Волна {waveNumber}");
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
        _spawnCoroutine = StartCoroutine(SpawnWave());
        Debug.Log($"[EnemySpawner] Волна {waveNumber} началась!");
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

    IEnumerator SpawnWave()
    {
        int normalCount = waveNumber;
        int armoredCount = waveNumber;

        normalCount += baseNormalEnemies - 1;
        armoredCount += baseArmoredEnemies - 1;

        Debug.Log($"[EnemySpawner] Волна {waveNumber}: {normalCount} обычных, {armoredCount} бронированных");

        // 1. Сначала спавним всех бронированных орков
        for (int i = 0; i < armoredCount; i++)
        {
            if (armoredEnemyPrefab != null)
            {
                Debug.Log($"[EnemySpawner] Спавн бронированного орка {i + 1}/{armoredCount}");
                Instantiate(armoredEnemyPrefab, transform.position, Quaternion.identity);

                // Ждём перед следующим врагом
                yield return new WaitForSeconds(spawnInterval * 2f); // Бронированные реже
            }
        }

        // 2. Потом спавним обычных орков
        Debug.Log($"[EnemySpawner] Спавн {normalCount} обычных орков");

        for (int i = 0; i < normalCount; i++)
        {
            GameObject normalEnemy = Instantiate(normalEnemyPrefab, transform.position, Quaternion.identity);

            // Инициализируем компонент у каждого врага
            TestEnemy enemyScript = normalEnemy.GetComponent<TestEnemy>();
            if (enemyScript != null)
            {
                enemyScript.startDelay = i * spawnInterval;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        _isSpawning = false;

        // Готовим следующую волну
        PrepareNextWave();
        Debug.Log($"[EnemySpawner] Волна {waveNumber} закончена!");
    }

    /// <summary>
    /// Подготовка следующей волны
    /// </summary>
    void PrepareNextWave()
    {
        waveNumber++;
        Debug.Log($"[EnemySpawner] Следующая волна: {waveNumber}. " +
                 $"Орков: {baseNormalEnemies + (waveNumber - 1)}, " +
                 $"Бронированных: {baseArmoredEnemies + (waveNumber - 1)}");
    }

    /// <summary>
    /// Сброс к первой волне (при рестарте игры)
    /// </summary>
    public void ResetWaves()
    {
        waveNumber = 1;
        _isSpawning = false;
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }
        Debug.Log("[EnemySpawner] Волны сброшены до 1");
    }
}