using UnityEngine;


[System.Serializable]
public class TowerShopEntry
{
    public string displayName = "Башня";
    public GameObject towerPrefab;
    public int cost = 50;
    public Sprite icon;
    [Tooltip("Опционально: курсор при выборе этой башни")]
    public Texture2D cursorIcon;
    public Vector2 cursorHotspot = new Vector2(16, 16);
}
