using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerLevelBalance", menuName = "Last Stronghold/Tower Level Balance")]
public class TowerLevelBalance : ScriptableObject
{
    [Header("Defaults (2 upgrade steps)")]
    [Tooltip("Если для башни нет записи в towerUpgradePrices")]
    public int defaultFirstUpgradeCost = 30;
    public int defaultSecondUpgradeCost = 50;

    [Tooltip("Цены 1-го и 2-го улучшения по ключу башни")]
    public TowerUpgradePriceEntry[] towerUpgradePrices;

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
}
