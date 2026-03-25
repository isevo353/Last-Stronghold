using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Панель магазина башен. Кнопки по типам башен; при достатке денег — выбор башни, переход в режим размещения.
/// </summary>
public class TowerShopUI : MonoBehaviour
{
    [Header("Tower types")]
    public List<TowerShopEntry> towers = new List<TowerShopEntry>();

    [Header("UI")]
    public Transform buttonsRoot;
    public GameObject buttonPrefab;

    private readonly List<Button> _buttons = new List<Button>();
    private readonly List<TowerShopEntry> _entries = new List<TowerShopEntry>();

    void Start()
    {
        if (buttonsRoot == null) buttonsRoot = transform;
        EnsureLayout();
        BuildButtons();
    }

    void EnsureLayout()
    {
        if (buttonsRoot == null) return;
        var lg = buttonsRoot.GetComponent<HorizontalLayoutGroup>();
        if (lg == null)
        {
            var h = buttonsRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 8;
            h.childAlignment = TextAnchor.MiddleCenter;
            h.childControlWidth = h.childControlHeight = true;
        }
    }

    void Update()
    {
        RefreshButtonStates();
    }

    void BuildButtons()
    {
        foreach (Transform c in buttonsRoot)
            Destroy(c.gameObject);
        _buttons.Clear();
        _entries.Clear();

        if (towers == null || towers.Count == 0)
        {
            Debug.LogWarning("[TowerShopUI] Список towers пуст. Добавь TowerShopEntry в инспекторе.");
            return;
        }
        for (int i = 0; i < towers.Count; i++)
        {
            var e = towers[i];
            if (e.towerPrefab == null) continue;

            GameObject go = buttonPrefab != null
                ? Instantiate(buttonPrefab, buttonsRoot)
                : CreateDefaultButton(buttonsRoot);
            var b = go.GetComponent<Button>();
            if (b == null) b = go.AddComponent<Button>();

            var icon = go.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null && e.icon != null) icon.sprite = e.icon;
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = $"{e.displayName}\n{e.cost}";

            var entry = e;
            b.onClick.AddListener(() => OnTowerClicked(entry));
            _buttons.Add(b);
            _entries.Add(e);
        }
    }

    GameObject CreateDefaultButton(Transform parent)
    {
        var go = new GameObject("TowerBtn");
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 80);
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 120;
        le.preferredHeight = 80;
        var img = go.AddComponent<Image>();
        img.color = new Color(0.25f, 0.25f, 0.3f, 0.95f);
        var btn = go.AddComponent<Button>();
        var c = btn.colors;
        c.highlightedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
        c.pressedColor = new Color(0.2f, 0.2f, 0.3f, 1f);
        btn.colors = c;

        var lab = new GameObject("Label");
        lab.transform.SetParent(go.transform, false);
        var lr = lab.AddComponent<RectTransform>();
        lr.anchorMin = Vector2.zero; lr.anchorMax = Vector2.one; lr.offsetMin = Vector2.zero; lr.offsetMax = Vector2.zero;
        var tmp = lab.AddComponent<TextMeshProUGUI>();
        tmp.text = "50";
        tmp.fontSize = 14;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    void OnTowerClicked(TowerShopEntry e)
    {
        if (e.towerPrefab == null) return;
        if (GameManager.Instance != null && GameManager.Instance.GetMoney() < e.cost) return;
        if (TowerPlacer.Instance != null)
            TowerPlacer.Instance.SelectTower(e.towerPrefab, e.cost, e.cursorIcon, e.cursorHotspot);
    }

    void RefreshButtonStates()
    {
        int money = GameManager.Instance != null ? GameManager.Instance.GetMoney() : 0;
        for (int i = 0; i < _entries.Count; i++)
        {
            if (i >= _buttons.Count) break;
            _buttons[i].interactable = money >= _entries[i].cost;
        }
    }
}
