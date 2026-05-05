using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CampaignSettings", menuName = "LastStronghold/Campaign Settings")]
public class CampaignSettings : ScriptableObject
{
    [Serializable]
    public struct BalanceData
    {
        public int startMoney;
        public float towerCostMultiplier;
        public float enemyRewardMultiplier;
        public float enemyHealthMultiplier;
        public float enemySpeedMultiplier;
        public float enemyCountMultiplier;
    }

    [Serializable]
    public class LevelSettings
    {
        public string sceneName;
        public int maxWavesToWin = 10;
        public string victoryTitle = "ПОБЕДА";
        public string victorySubtitle = "Вы прошли уровень!";

        [Header("Balance")]
        [Tooltip("Если <= 0, используется defaultStartMoney")]
        public int startMoneyOverride = -1;

        [Tooltip("Множитель стоимости башен в магазине")]
        public float towerCostMultiplier = 1f;

        [Tooltip("Множитель награды за убийство врагов")]
        public float enemyRewardMultiplier = 1f;

        [Tooltip("Множитель максимального HP врагов")]
        public float enemyHealthMultiplier = 1f;

        [Tooltip("Множитель скорости врагов")]
        public float enemySpeedMultiplier = 1f;

        [Tooltip("Множитель количества врагов в волнах")]
        public float enemyCountMultiplier = 1f;

        [Header("Enemy counts per wave (base)")]
        [Tooltip("Slime count at waveNumber=1")]
        public int slimeCountWave1 = 5;
        [Tooltip("Slime count growth per wave")]
        public int slimeGrowthPerWave = 2; // 5,7,9,11...

        [Tooltip("Orc count at waveNumber=1")]
        public int normalCountWave1 = 3;
        [Tooltip("Orc count growth per wave")]
        public int normalGrowthPerWave = 1; // 3,4,5,6...

        [Tooltip("Armored start wave (inclusive)")]
        public int armoredStartWave = 1;
        [Tooltip("Armored count at start wave")]
        public int armoredCountAtStart = 1; 
        [Tooltip("Increase armored count every N waves (integer division)")]
        public int armoredIntervalWaves = 2;
        [Tooltip("Armored count step per interval")]
        public int armoredGrowthStep = 1;

        [Tooltip("Skeleton start wave (inclusive)")]
        public int skeletonStartWave = 3;
        [Tooltip("Skeleton count at start wave")]
        public int skeletonCountAtStart = 1; 
        [Tooltip("Increase skeleton count every N waves (integer division)")]
        public int skeletonIntervalWaves = 2; 
        [Tooltip("Skeleton count step per interval")]
        public int skeletonGrowthStep = 1;

        [Header("Towers (optional)")]
        [Tooltip("Свой ассет цен улучшений для этой сцены. Пусто — берутся поля Tower upgrades из CampaignSettings.")]
        public TowerLevelBalance towerLevelBalance;
    }

    public int defaultMaxWavesToWin = 10;
    public string defaultVictoryTitle = "ПОБЕДА";
    public string defaultVictorySubtitle = "Вы прошли 10 волн!";

    [Header("Default balance")]
    public int defaultStartMoney = 80;
    public float defaultTowerCostMultiplier = 1f;
    public float defaultEnemyRewardMultiplier = 1f;
    public float defaultEnemyHealthMultiplier = 1f;
    public float defaultEnemySpeedMultiplier = 1f;
    public float defaultEnemyCountMultiplier = 1f;

    [Header("Tower upgrades (2 steps)")]
    [Tooltip("Если для башни нет записи в towerUpgradePrices, используются эти цены")]
    public int defaultFirstUpgradeCost = 30;
    public int defaultSecondUpgradeCost = 50;

    [Tooltip("Глобальные цены (если у уровня не задан Tower Level Balance)")]
    public TowerUpgradePriceEntry[] towerUpgradePrices;

    public LevelSettings[] levelSettings;
    public LevelSettings GetSettingsForScene(string sceneName)
    {
        if (levelSettings == null) return null;
        for (int i = 0; i < levelSettings.Length; i++)
        {
            LevelSettings settings = levelSettings[i];
            if (settings != null && settings.sceneName == sceneName)
            {
                return settings;
            }
        }
        return null;
    }

    public BalanceData GetBalanceForScene(string sceneName)
    {
        LevelSettings level = GetSettingsForScene(sceneName);
        BalanceData data = new BalanceData
        {
            startMoney = defaultStartMoney,
            towerCostMultiplier = defaultTowerCostMultiplier,
            enemyRewardMultiplier = defaultEnemyRewardMultiplier,
            enemyHealthMultiplier = defaultEnemyHealthMultiplier,
            enemySpeedMultiplier = defaultEnemySpeedMultiplier,
            enemyCountMultiplier = defaultEnemyCountMultiplier,
        };

        if (level == null) return data;

        if (level.startMoneyOverride > 0) data.startMoney = level.startMoneyOverride;
        data.towerCostMultiplier = level.towerCostMultiplier;
        data.enemyRewardMultiplier = level.enemyRewardMultiplier;
        data.enemyHealthMultiplier = level.enemyHealthMultiplier;
        data.enemySpeedMultiplier = level.enemySpeedMultiplier;
        data.enemyCountMultiplier = level.enemyCountMultiplier;
        return data;
    }

    /// <summary>Две цены улучшения для башни с ключом towerKey (совпадение без учёта регистра).</summary>
    public int[] GetUpgradeCostsForTower(string towerKey)
    {
        int a = Mathf.Max(0, defaultFirstUpgradeCost);
        int b = Mathf.Max(0, defaultSecondUpgradeCost);

        if (!string.IsNullOrEmpty(towerKey) && towerUpgradePrices != null)
        {
            for (int i = 0; i < towerUpgradePrices.Length; i++)
            {
                TowerUpgradePriceEntry e = towerUpgradePrices[i];
                if (e == null || string.IsNullOrEmpty(e.towerKey))
                    continue;
                if (string.Equals(e.towerKey, towerKey, StringComparison.OrdinalIgnoreCase))
                {
                    a = Mathf.Max(0, e.firstUpgradeCost);
                    b = Mathf.Max(0, e.secondUpgradeCost);
                    break;
                }
            }
        }

        return new int[2] { a, b };
    }

    /// <summary>
    /// Учитывает <see cref="LevelSettings.towerLevelBalance"/> для текущей сцены, иначе глобальные цены.
    /// </summary>
    public int[] GetUpgradeCostsForSceneTower(string sceneName, string towerKey)
    {
        LevelSettings lev = GetSettingsForScene(sceneName);
        if (lev != null && lev.towerLevelBalance != null)
            return lev.towerLevelBalance.GetUpgradeCostsForTower(towerKey);
        return GetUpgradeCostsForTower(towerKey);
    }
}
