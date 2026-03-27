using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
    private CampaignSettings.BalanceData _balance;
    private CampaignSettings.LevelSettings _levelSettings;

    public Action onWaveFinished;

    void Start()
    {
        waveNumber = 1;
        ApplyCampaignBalance();
    }

    void ApplyCampaignBalance()
    {
        CampaignSettings settings = Resources.Load<CampaignSettings>("CampaignSettings");
        if (settings != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            _balance = settings.GetBalanceForScene(sceneName);
            _levelSettings = settings.GetSettingsForScene(sceneName);
        }
        else
        {
            _balance = new CampaignSettings.BalanceData
            {
                startMoney = 0,
                towerCostMultiplier = 1f,
                enemyRewardMultiplier = 1f,
                enemyHealthMultiplier = 1f,
                enemySpeedMultiplier = 1f,
                enemyCountMultiplier = 1f
            };
            _levelSettings = null;
        }
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

    GameObject TrySpawn(GameObject prefab, string enemyName)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[EnemySpawner] Пропущен спавн '{enemyName}': prefab не назначен.");
            return null;
        }

        GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);
        ApplyEnemyBalance(go);
        return go;
    }

    void ApplyEnemyBalance(GameObject go)
    {
        if (go == null) return;
        TestEnemy e = go.GetComponent<TestEnemy>();
        if (e == null) return;

        float hpMult = Mathf.Max(0.1f, _balance.enemyHealthMultiplier);
        float speedMult = Mathf.Max(0.1f, _balance.enemySpeedMultiplier);
        float rewardMult = Mathf.Max(0f, _balance.enemyRewardMultiplier);

        e.maxHealth = Mathf.Max(1, Mathf.RoundToInt(e.maxHealth * hpMult));
        e.currentHealth = e.maxHealth;
        e.speed = e.speed * speedMult;
        e.rewardMoney = Mathf.Max(0, Mathf.RoundToInt(e.rewardMoney * rewardMult));
    }

    int ScaleCount(int count)
    {
        float mult = Mathf.Max(0.1f, _balance.enemyCountMultiplier);
        return Mathf.Max(0, Mathf.CeilToInt(count * mult));
    }

    IEnumerator SpawnWave()
    {
        // Базовые количества врагов берём из CampaignSettings (не хардкод в коде).
        // Потом дополнительно масштабируем через _balance.enemyCountMultiplier.
        int slimeBase = _levelSettings != null
            ? Mathf.Max(0, _levelSettings.slimeCountWave1 + (waveNumber - 1) * _levelSettings.slimeGrowthPerWave)
            : Mathf.Max(0, 5 + (waveNumber - 1) * 2);

        int normalBase = _levelSettings != null
            ? Mathf.Max(0, _levelSettings.normalCountWave1 + (waveNumber - 1) * _levelSettings.normalGrowthPerWave)
            : Mathf.Max(0, 3 + (waveNumber - 1) * 1);

        int armoredBase = 0;
        if (_levelSettings != null)
        {
            if (waveNumber >= _levelSettings.armoredStartWave)
            {
                int interval = Mathf.Max(1, _levelSettings.armoredIntervalWaves);
                int steps = (waveNumber - _levelSettings.armoredStartWave) / interval;
                armoredBase = Mathf.Max(0, _levelSettings.armoredCountAtStart + steps * _levelSettings.armoredGrowthStep);
            }
        }
        else
        {
            armoredBase = Mathf.Max(0, 1 + (waveNumber - 1) / 2);
        }

        int skeletonBase = 0;
        if (_levelSettings != null)
        {
            if (waveNumber >= _levelSettings.skeletonStartWave)
            {
                int interval = Mathf.Max(1, _levelSettings.skeletonIntervalWaves);
                int steps = (waveNumber - _levelSettings.skeletonStartWave) / interval;
                skeletonBase = Mathf.Max(0, _levelSettings.skeletonCountAtStart + steps * _levelSettings.skeletonGrowthStep);
            }
        }
        else
        {
            skeletonBase = Mathf.Max(0, waveNumber >= 3 ? 1 + (waveNumber - 3) / 2 : 0);
        }

        int slimeCount = ScaleCount(slimeBase);
        int normalCount = ScaleCount(normalBase);
        int armoredCount = ScaleCount(armoredBase);
        int skeletonCount = ScaleCount(skeletonBase);

        // ПОРЯДОК СПАВНА:
        // 1. Слизни (быстро, интервал 0.5)
        for (int i = 0; i < slimeCount; i++)
        {
            TrySpawn(slimeEnemyPrefab, "Slime");
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f); // пауза между группами

        // 2. Обычные орки (интервал 1)
        for (int i = 0; i < normalCount; i++)
        {
            TrySpawn(normalEnemyPrefab, "Orc");
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.5f);

        // 3. Бронированные орки (интервал 1.5)
        for (int i = 0; i < armoredCount; i++)
        {
            TrySpawn(armoredEnemyPrefab, "ArmedOrc");
            yield return new WaitForSeconds(1.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // 4. Скелеты (интервал 2, идут последними)
        for (int i = 0; i < skeletonCount; i++)
        {
            TrySpawn(skeletonEnemyPrefab, "Skeleton");
            yield return new WaitForSeconds(2f);
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