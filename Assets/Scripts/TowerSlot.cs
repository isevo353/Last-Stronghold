using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TowerSlot : MonoBehaviour
{
    [Header("Tower Settings")]
    public GameObject towerPrefab;
    public int towerCost = 50;

    [Header("Visuals")]
    public SpriteRenderer slotSprite;
    public TextMeshProUGUI costText;

    private bool isOccupied = false;
    private Color originalColor;

    void Start()
    {
        if (slotSprite == null)
            slotSprite = GetComponent<SpriteRenderer>();

        originalColor = slotSprite.color;

        // Показываем цену
        if (costText != null)
        {
            costText.text = towerCost.ToString();
            UpdateSlotColor();
        }
    }

    void Update()
    {
        // Обновляем цвет в зависимости от денег
        UpdateSlotColor();
    }

    void UpdateSlotColor()
    {
        if (isOccupied)
        {
            slotSprite.color = Color.gray;

            if (costText != null && costText.gameObject.activeSelf)
            {
                costText.gameObject.SetActive(false);
                Debug.Log("Скрыл цену для занятого слота");
            }
        }
        else
        {
            bool canAfford = GameManager.Instance != null &&
                            GameManager.Instance.GetMoney() >= towerCost;

            slotSprite.color = canAfford ? Color.green : Color.red;

            if (costText != null && !costText.gameObject.activeSelf)
            {
                costText.gameObject.SetActive(true);
            }

            if (costText != null)
            {
                costText.color = canAfford ? Color.green : Color.red;
            }
        }
    }
    void OnMouseDown()
    {
        if (!isOccupied && GameManager.Instance != null)
        {
            if (GameManager.Instance.TrySpendMoney(towerCost))
            {
                // Ставим башню
                Instantiate(towerPrefab, transform.position, Quaternion.identity);
                isOccupied = true;
                UpdateSlotColor();

                Debug.Log($"Башня построена за {towerCost} монет!");
            }
        }
    }
}