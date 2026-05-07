using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Панель по ПКМ на башне: продать и улучшить, с отображением цен.
/// Позиция панели — рядом с курсором (место ПКМ).
/// </summary>
public class TowerContextPanel : MonoBehaviour
{
    public static TowerContextPanel Instance { get; private set; }

    [Header("Optional: если пусто — UI создаётся на первом Canvas в сцене")]
    public RectTransform panelRoot;
    public Button sellButton;
    public Button upgradeButton;
    public Text sellLabel;
    public Text upgradeLabel;

    Canvas _canvas;
    Tower _current;
    int _openedFrame = -1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panelRoot == null || sellButton == null || upgradeButton == null || sellLabel == null || upgradeLabel == null)
            BuildRuntimeUi();

        if (_canvas == null && panelRoot != null)
            _canvas = panelRoot.GetComponentInParent<Canvas>();

        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellClicked);
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        SetVisible(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (panelRoot == null || !panelRoot.gameObject.activeSelf || _current == null)
            return;

        if (Time.frameCount != _openedFrame && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        if (Time.frameCount != _openedFrame && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            Camera uiCam = GetUICameraForCanvas(_canvas);
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelRoot, Input.mousePosition, uiCam))
                Close();
        }

        RefreshLabels();
    }

    public static TowerContextPanel EnsureExists()
    {
        if (Instance != null)
            return Instance;
        var host = new GameObject("TowerContextPanel");
        host.AddComponent<TowerContextPanel>();
        return Instance;
    }

    public void Open(Tower tower)
    {
        OpenAtScreenPosition(tower, Input.mousePosition);
    }

    public void OpenAtScreenPosition(Tower tower, Vector2 screenPosition)
    {
        if (tower == null)
        {
            Close();
            return;
        }
        if (panelRoot == null)
        {
            Debug.LogWarning("[TowerContextPanel] Panel root is missing, cannot open context menu.");
            return;
        }

        _current = tower;
        _openedFrame = Time.frameCount;
        ApplyCurrentButtonsVisibility();
        SetVisible(true);
        EnsurePanelDrawsOnTop();
        RefreshLabels();
        PositionNearTower();
    }

    public static void ShowForTower(Tower tower)
    {
        ShowForTowerAtScreenPosition(tower, Input.mousePosition);
    }

    public static void ShowForTowerAtScreenPosition(Tower tower, Vector2 screenPosition)
    {
        EnsureExists();
        if (Instance != null)
            Instance.OpenAtScreenPosition(tower, screenPosition);
    }

    public static void HidePanel()
    {
        if (Instance != null)
            Instance.Close();
    }

    public void Close()
    {
        _current = null;
        SetVisible(false);
    }

    void SetVisible(bool v)
    {
        if (panelRoot != null)
            panelRoot.gameObject.SetActive(v);
    }

    void PositionNearScreenPoint(Vector2 screenPoint)
    {
        if (panelRoot == null)
            return;

        Canvas canvas = panelRoot.GetComponentInParent<Canvas>();
        if (canvas != null)
            canvas = canvas.rootCanvas;
        if (canvas == null)
            canvas = _canvas;
        if (canvas == null)
            return;
        _canvas = canvas;

        RectTransform canvasRt = canvas.transform as RectTransform;
        if (canvasRt == null)
            return;

        Camera uiCam = GetUICameraForCanvas(canvas);

        panelRoot.anchorMin = panelRoot.anchorMax = new Vector2(0.5f, 0.5f);
        panelRoot.pivot = new Vector2(0f, 1f);

        const float pad = 12f;
        Vector2 pivotScreen = new Vector2(screenPoint.x + pad, screenPoint.y + pad);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRt, pivotScreen, uiCam, out Vector2 localPt);
        panelRoot.anchoredPosition = localPt;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRoot);

        float width = panelRoot.rect.width;
        float height = panelRoot.rect.height;
        Rect r = canvasRt.rect;
        const float edgePad = 8f;
        float minX = r.xMin + edgePad;
        float maxX = r.xMax - width - edgePad;
        float minY = r.yMin + height + edgePad;
        float maxY = r.yMax - edgePad;

        panelRoot.anchoredPosition = new Vector2(
            Mathf.Clamp(panelRoot.anchoredPosition.x, minX, maxX),
            Mathf.Clamp(panelRoot.anchoredPosition.y, minY, maxY)
        );
    }

    static Camera GetGameViewCamera()
    {
        if (TowerPlacer.Instance != null && TowerPlacer.Instance.cam != null)
            return TowerPlacer.Instance.cam;
        return Camera.main;
    }

    static Camera GetUICameraForCanvas(Canvas canvas)
    {
        if (canvas == null)
            return null;
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

    void RefreshLabels()
    {
        if (_current == null || upgradeLabel == null || upgradeButton == null)
            return;

        if (!_current.IsBuilderHouse && sellLabel != null)
        {
            int sellRefund = _current.GetSellRefund();
            sellLabel.text = $"Продать (+{sellRefund})";
        }

        if (_current.CanUpgradeUnlocked(out string blockedByBuilder))
        {
            int upCost = _current.GetUpgradeCost();
            upgradeLabel.text = $"Улучшить ({upCost})";
            bool canPay = GameManager.Instance != null && GameManager.Instance.GetMoney() >= upCost;
            upgradeButton.interactable = canPay;
        }
        else
        {
            upgradeLabel.text = string.IsNullOrEmpty(blockedByBuilder) ? "Макс. уровень" : blockedByBuilder;
            upgradeButton.interactable = false;
        }
    }

    void OnSellClicked()
    {
        if (_current == null)
            return;
        if (_current.IsBuilderHouse)
            return;
        Tower t = _current;
        Close();
        t.SellAndRemove();
    }

    /// <summary>
    /// Рисует панель поверх остального UI и спрайтов (Screen Space — Camera / Overlay).
    /// </summary>
    void EnsurePanelDrawsOnTop()
    {
        if (panelRoot == null)
            return;
        Canvas popup = panelRoot.GetComponent<Canvas>();
        if (popup == null)
            popup = panelRoot.gameObject.AddComponent<Canvas>();
        popup.overrideSorting = true;
        popup.sortingOrder = 32700;

        if (panelRoot.GetComponent<GraphicRaycaster>() == null)
            panelRoot.gameObject.AddComponent<GraphicRaycaster>();

        panelRoot.SetAsLastSibling();
    }

    void OnUpgradeClicked()
    {
        if (_current == null)
            return;
        _current.TryUpgrade();
        RefreshLabels();
        PositionNearTower();
    }

    void ApplyCurrentButtonsVisibility()
    {
        if (_current == null || sellButton == null || panelRoot == null)
            return;

        bool showSell = !_current.IsBuilderHouse;
        sellButton.gameObject.SetActive(showSell);
        panelRoot.sizeDelta = showSell ? new Vector2(200f, 88f) : new Vector2(200f, 48f);
    }

    void PositionNearTower()
    {
        if (_current == null)
            return;

        Camera cam = GetGameViewCamera();
        if (cam == null)
            return;

        Vector3 world = _current.transform.position;
        Vector2 towerScreen = RectTransformUtility.WorldToScreenPoint(cam, world);
        PositionNearScreenPoint(towerScreen);
    }

    void BuildRuntimeUi()
    {
        _canvas = FindPrimaryCanvas();
        if (_canvas == null)
        {
            Debug.LogError("[TowerContextPanel] В сцене нет Canvas — добавь Canvas или задай ссылки в инспекторе.");
            return;
        }

        var root = new GameObject("TowerActionPanel");
        root.transform.SetParent(_canvas.transform, false);
        panelRoot = root.AddComponent<RectTransform>();
        panelRoot.anchorMin = panelRoot.anchorMax = new Vector2(0.5f, 0.5f);
        panelRoot.pivot = new Vector2(0f, 1f);
        panelRoot.sizeDelta = new Vector2(200f, 88f);
        panelRoot.gameObject.SetActive(false);

        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.12f, 0.12f, 0.14f, 0.94f);

        var v = root.AddComponent<VerticalLayoutGroup>();
        v.padding = new RectOffset(8, 8, 8, 8);
        v.spacing = 6f;
        v.childAlignment = TextAnchor.UpperCenter;
        v.childControlHeight = true;
        v.childControlWidth = true;
        v.childForceExpandHeight = false;
        v.childForceExpandWidth = true;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            Debug.LogError("[TowerContextPanel] LegacyRuntime.ttf not found. Assign UI references in inspector or add a runtime font.");

        sellButton = CreateButton(root.transform, "SellButton", font, new Color(0.25f, 0.45f, 0.28f, 1f), out sellLabel);
        upgradeButton = CreateButton(root.transform, "UpgradeButton", font, new Color(0.22f, 0.35f, 0.55f, 1f), out upgradeLabel);

        var le = root.AddComponent<LayoutElement>();
        le.minWidth = 200f;
        le.preferredWidth = 200f;

        EnsurePanelDrawsOnTop();
    }

    static Button CreateButton(Transform parent, string name, Font font, Color bg, out Text label)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = bg;
        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.35f, 0.6f, 0.4f);
        colors.pressedColor = new Color(0.2f, 0.35f, 0.22f);
        btn.colors = colors;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        label = textGo.AddComponent<Text>();
        label.font = font;
        label.fontSize = 14;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 10;
        label.resizeTextMaxSize = 16;

        var textRt = label.rectTransform;
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(4f, 2f);
        textRt.offsetMax = new Vector2(-4f, -2f);

        var layout = go.AddComponent<LayoutElement>();
        layout.minHeight = 32f;
        layout.preferredHeight = 34f;

        return btn;
    }

    static Canvas FindPrimaryCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Canvas best = null;
        int bestScore = int.MinValue;
        foreach (var c in canvases)
        {
            if (c == null || !c.isActiveAndEnabled)
                continue;
            int score = c.sortingOrder;
            float area = c.pixelRect.width * c.pixelRect.height;
            if (c.isRootCanvas)
                score += 5000;
            if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                score += 2000;
            if (area > 100f)
                score += Mathf.RoundToInt(Mathf.Min(area * 0.01f, 5000f));
            if (c.gameObject.name.IndexOf("pause", System.StringComparison.OrdinalIgnoreCase) >= 0)
                score -= 20000;

            if (score > bestScore)
            {
                bestScore = score;
                best = c;
            }
        }

        return best != null ? best.rootCanvas : FindObjectOfType<Canvas>();
    }
}
