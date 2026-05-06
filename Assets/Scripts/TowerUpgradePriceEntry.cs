using System;
using UnityEngine;

[Serializable]
public class TowerUpgradePriceEntry
{
    [Tooltip("Имя префаба башни (без .prefab) или Tower.campaignTowerId")]
    public string towerKey = "";
    public int firstUpgradeCost = 30;
    public int secondUpgradeCost = 50;
}
