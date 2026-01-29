using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Режим размещения башни: выбор в магазине → курсор/призрак → клик по клетке.
/// </summary>
public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance { get; private set; }

    [Header("References")]
    public Camera cam;
    public LayerMask placementLayer = -1;

    [Header("Ghost")]
    public GameObject ghostRoot;
    public SpriteRenderer ghostSprite;

    private GameObject _selectedPrefab;
    private int _selectedCost;
    private Texture2D _cursorIcon;
    private Vector2 _cursorHotspot;
    private TowerSlot _lastHighlighted;
    public bool IsPlacementMode => _selectedPrefab != null;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (cam == null) cam = Camera.main;
        EnsureGhost();
    }

    void Update()
    {
        if (!IsPlacementMode)
        {
            ClearAllHighlights();
            return;
        }

        if (PauseMenuController.IsPaused) return;

        Vector2 world = GetMouseWorld();
        if (ghostRoot != null)
        {
            ghostRoot.SetActive(true);
            ghostRoot.transform.position = new Vector3(world.x, world.y, 0f);
        }

        TowerSlot cell = GetCellUnderMouse();
        ClearAllHighlights();
        bool canAfford = GameManager.Instance != null && GameManager.Instance.GetMoney() >= _selectedCost;
        if (cell != null && !cell.IsOccupied)
        {
            cell.SetHighlight(canAfford);
            _lastHighlighted = cell;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            if (cell != null && !cell.IsOccupied && canAfford && cell.TryPlace(_selectedPrefab, _selectedCost))
            {
                ClearSelection();
                return;
            }
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            ClearSelection();
    }

    public void SelectTower(GameObject prefab, int cost, Texture2D cursorIcon = null, Vector2? cursorHotspot = null)
    {
        if (prefab == null) return;
        _selectedPrefab = prefab;
        _selectedCost = cost;
        _cursorIcon = cursorIcon;
        _cursorHotspot = cursorHotspot ?? new Vector2(16, 16);

        if (ghostSprite != null)
        {
            var sr = prefab.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
                ghostSprite.sprite = sr.sprite;
        }
        if (_cursorIcon != null)
            Cursor.SetCursor(_cursorIcon, _cursorHotspot, CursorMode.Auto);
    }

    public void ClearSelection()
    {
        _selectedPrefab = null;
        _selectedCost = 0;
        _cursorIcon = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        if (ghostRoot != null) ghostRoot.SetActive(false);
        ClearAllHighlights();
    }

    Vector2 GetMouseWorld()
    {
        if (cam == null) return Vector2.zero;
        var p = Input.mousePosition;
        p.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(p);
    }

    TowerSlot GetCellUnderMouse()
    {
        Vector2 w = GetMouseWorld();
        var c = Physics2D.OverlapPoint(w, placementLayer);
        return c != null ? c.GetComponent<TowerSlot>() : null;
    }

    void ClearAllHighlights()
    {
        if (_lastHighlighted != null) { _lastHighlighted.ClearHighlight(); _lastHighlighted = null; }
    }

    void EnsureGhost()
    {
        if (ghostRoot != null) return;
        ghostRoot = new GameObject("TowerPlacerGhost");
        ghostRoot.transform.SetParent(transform);
        var go = new GameObject("Sprite");
        go.transform.SetParent(ghostRoot.transform, false);
        ghostSprite = go.AddComponent<SpriteRenderer>();
        ghostSprite.sortingOrder = 100;
        ghostSprite.color = new Color(1f, 1f, 1f, 0.6f);
        ghostRoot.SetActive(false);
    }
}
