using System;
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalEnemyPrefab;
    public GameObject armoredEnemyPrefab;
    public GameObject slimeEnemyPrefab;
    public GameObject skeletonEnemyPrefab;

    [Header("Wave Settings")]
    public int waveNumber = 1;
    public float spawnInterval = 1f;

    private Coroutine _spawnCoroutine;
    private bool _isSpawning = false;

    public Action onWaveFinished;

    void Start()
    {
        waveNumber = 1;
    }

    public void StartSpawning()
    {
        if (_isSpawning) return;
        _isSpawning = true;
        _spawnCoroutine = StartCoroutine(SpawnWave());
    }

    public void StopSpawning()
    {
        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
        _isSpawning = false;
    }

    IEnumerator SpawnWave()
    {
        // Расчёт количества
        int slimeCount = 5 + (waveNumber - 1) * 2;           // 5,7,9,11...
        int normalCount = 3 + (waveNumber - 1);               // 3,4,5,6...
        int armoredCount = 1 + (waveNumber - 1) / 2;          // 1,1,2,2,3...
        int skeletonCount = waveNumber >= 3 ? 1 + (waveNumber - 3) / 2 : 0; // с 3й волны: 1,1,2,2...

        // ПОРЯДОК СПАВНА:
        // 1. Слизни (быстро, интервал 0.5)
        if (slimeEnemyPrefab != null)
        {
            for (int i = 0; i < slimeCount; i++)
            {
                Instantiate(slimeEnemyPrefab, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] slimeEnemyPrefab не назначен!");
        }

        yield return new WaitForSeconds(0.5f); // пауза между группами

        // 2. Обычные орки (интервал 1)
        if (normalEnemyPrefab != null)
        {
            for (int i = 0; i < normalCount; i++)
            {
                Instantiate(normalEnemyPrefab, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] normalEnemyPrefab не назначен!");
        }

        yield return new WaitForSeconds(0.5f);

        // 3. Бронированные орки (интервал 1.5)
        if (armoredEnemyPrefab != null)
        {
            for (int i = 0; i < armoredCount; i++)
            {
                Instantiate(armoredEnemyPrefab, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(1.5f);
            }
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] armoredEnemyPrefab не назначен!");
        }

        yield return new WaitForSeconds(0.5f);

        // 4. Скелеты (интервал 2, идут последними)
        if (skeletonCount > 0)
        {
            if (skeletonEnemyPrefab != null)
            {
                for (int i = 0; i < skeletonCount; i++)
                {
                    Instantiate(skeletonEnemyPrefab, transform.position, Quaternion.identity);
                    yield return new WaitForSeconds(2f);
                }
            }
            else
            {
                Debug.LogWarning("[EnemySpawner] skeletonEnemyPrefab не назначен! Скелеты пропущены.");
            }
        }

        _isSpawning = false;
        waveNumber++;
        onWaveFinished?.Invoke();
    }

    public void ResetWaves()
    {
        waveNumber = 1;
        _isSpawning = false;
        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
    }
}