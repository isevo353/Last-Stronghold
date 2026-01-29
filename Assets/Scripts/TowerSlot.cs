using UnityEngine;
using TMPro;

/// <summary>
/// Клетка для размещения башни. Размещение через магазин (TowerShopUI) + TowerPlacer.
/// </summary>
public class TowerSlot : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer slotSprite;
    public TextMeshProUGUI costText;
    public Color colorCanPlace = new Color(0.2f, 0.8f, 0.2f, 0.8f);
    public Color colorOccupied = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    public Color colorDisabled = new Color(0.8f, 0.2f, 0.2f, 0.6f);

    private bool _occupied;
    private Color _originalColor;

    public bool IsOccupied => _occupied;
    public Vector2 Position => transform.position;

    void Start()
    {
        if (slotSprite == null)
            slotSprite = GetComponent<SpriteRenderer>();
        if (slotSprite != null)
            _originalColor = slotSprite.color;
        if (costText != null)
            costText.gameObject.SetActive(false);
        ClearHighlight();
    }

    /// <summary>
    /// Вызывается из TowerPlacer при установке башни.
    /// </summary>
    public bool TryPlace(GameObject towerPrefab, int cost)
    {
        if (_occupied || towerPrefab == null) return false;
        if (GameManager.Instance == null || !GameManager.Instance.TrySpendMoney(cost))
            return false;

        Instantiate(towerPrefab, Position, Quaternion.identity);
        _occupied = true;
        ClearHighlight();
        return true;
    }

    public void SetHighlight(bool canPlace)
    {
        if (slotSprite == null || _occupied) return;
        slotSprite.color = canPlace ? colorCanPlace : colorDisabled;
    }

    public void ClearHighlight()
    {
        if (slotSprite == null) return;
        slotSprite.color = _occupied ? colorOccupied : _originalColor;
    }
}
