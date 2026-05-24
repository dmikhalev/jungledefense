using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }

    public GameObject selectedTowerPrefab;

    public bool IsBuildMode => selectedTowerPrefab != null;

    private Camera cam;
    private GameObject previewObject;
    private SpriteRenderer previewSpriteRenderer;
    private RangeCircleRenderer previewRangeCircle;
    private Tower selectedTowerTemplate;
    private Tile pendingTile;
    private bool isDraggingTower;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        cam = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (!IsBuildMode)
        {
            DestroyPreview();
            return;
        }

        if (isDraggingTower)
        {
            UpdateDragPreview();
            return;
        }

        UpdatePreview();

        if (InputHelper.TryGetTapBegan(out Vector2 screenPosition))
        {
            TryConfirmOrMovePreview(screenPosition);
        }
    }

    public void SelectTowerForBuilding(GameObject towerPrefab)
    {
        if (towerPrefab == null)
        {
            ClearSelection();
            return;
        }

        if (selectedTowerPrefab == towerPrefab)
        {
            ClearSelection();
            return;
        }

        SetSelectedTower(towerPrefab);
        pendingTile = null;
        CreatePreview();
    }

    public void BeginTowerDrag(GameObject towerPrefab)
    {
        if (towerPrefab == null)
        {
            ClearSelection();
            return;
        }

        SetSelectedTower(towerPrefab);
        pendingTile = null;
        isDraggingTower = true;
        CreatePreview();
        UpdateDragPreview();
    }

    public void EndTowerDrag(Vector2 screenPosition)
    {
        if (!isDraggingTower)
        {
            return;
        }

        isDraggingTower = false;

        Tile tile = GetTileAtScreenPosition(screenPosition);

        if (tile == null || !IsTileValidForPlacement(tile) || !CanAffordSelectedTower())
        {
            ClearSelection();
            return;
        }

        PlaceTower(tile);
    }

    public void ClearSelection()
    {
        selectedTowerPrefab = null;
        selectedTowerTemplate = null;
        pendingTile = null;
        isDraggingTower = false;

        DestroyPreview();
    }

    private void SetSelectedTower(GameObject towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;
        selectedTowerTemplate = selectedTowerPrefab.GetComponent<Tower>();

        if (selectedTowerTemplate == null)
        {
            Debug.LogError("Selected tower prefab has no Tower component.");
            ClearSelection();
        }
    }

    private void TryConfirmOrMovePreview(Vector2 screenPosition)
    {
        Tile tile = GetTileAtScreenPosition(screenPosition);

        if (tile == null || !tile.isBuildable || tile.isOccupied)
        {
            pendingTile = null;
            SetPreviewValid(false);
            return;
        }

        MovePreviewToTile(tile);

        bool canAfford = CanAffordSelectedTower();
        SetPreviewValid(canAfford);

        if (pendingTile != tile)
        {
            pendingTile = tile;
            return;
        }

        if (!canAfford)
        {
            return;
        }

        PlaceTower(tile);
    }

    private void PlaceTower(Tile tile)
    {
        if (selectedTowerPrefab == null || selectedTowerTemplate == null)
        {
            ClearSelection();
            return;
        }

        if (GameManager.Instance == null || !GameManager.Instance.SpendMoney(selectedTowerTemplate.cost))
        {
            return;
        }

        GameObject towerObject = Instantiate(
            selectedTowerPrefab,
            tile.transform.position,
            Quaternion.identity
        );

        Tower tower = towerObject.GetComponent<Tower>();

        if (tower != null)
        {
            tower.SetOccupiedTile(tile);
        }

        tile.isOccupied = true;

        ClearSelection();
    }

    private void UpdatePreview()
    {
        if (previewObject == null)
        {
            CreatePreview();
        }

        if (previewObject == null)
        {
            return;
        }

        if (pendingTile != null)
        {
            MovePreviewToTile(pendingTile);
            SetPreviewValid(IsTileValidForPlacement(pendingTile) && CanAffordSelectedTower());
            return;
        }

        if (!InputHelper.TryGetPointerScreenPosition(out Vector2 screenPosition))
        {
            return;
        }

        Tile tile = GetTileAtScreenPosition(screenPosition);

        if (tile != null)
        {
            MovePreviewToTile(tile);
            SetPreviewValid(IsTileValidForPlacement(tile) && CanAffordSelectedTower());
        }
        else
        {
            MovePreviewToWorldPosition(GetWorldPosition(screenPosition));
            SetPreviewValid(false);
        }
    }

    private void UpdateDragPreview()
    {
        if (previewObject == null)
        {
            CreatePreview();
        }

        if (!InputHelper.TryGetPointerScreenPosition(out Vector2 screenPosition))
        {
            return;
        }

        Tile tile = GetTileAtScreenPosition(screenPosition);

        if (tile != null)
        {
            MovePreviewToTile(tile);
            SetPreviewValid(IsTileValidForPlacement(tile) && CanAffordSelectedTower());
        }
        else
        {
            MovePreviewToWorldPosition(GetWorldPosition(screenPosition));
            SetPreviewValid(false);
        }
    }

    private void CreatePreview()
    {
        DestroyPreview();

        if (selectedTowerPrefab == null)
        {
            return;
        }

        previewObject = new GameObject("TowerBuildPreview");
        previewObject.transform.localScale = selectedTowerPrefab.transform.localScale;

        SpriteRenderer sourceSprite = selectedTowerPrefab.GetComponent<SpriteRenderer>();

        if (sourceSprite != null)
        {
            previewSpriteRenderer = previewObject.AddComponent<SpriteRenderer>();
            previewSpriteRenderer.sprite = sourceSprite.sprite;
            previewSpriteRenderer.sortingLayerID = sourceSprite.sortingLayerID;
            previewSpriteRenderer.sortingOrder = sourceSprite.sortingOrder + 100;
        }

        GameObject rangeObject = new GameObject("TowerBuildPreviewRange");
        previewRangeCircle = rangeObject.AddComponent<RangeCircleRenderer>();

        float range = selectedTowerTemplate != null ? selectedTowerTemplate.range : 1f;
        previewRangeCircle.Draw(range);

        SetPreviewValid(false);
    }

    private void DestroyPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
            previewSpriteRenderer = null;
        }

        if (previewRangeCircle != null)
        {
            Destroy(previewRangeCircle.gameObject);
            previewRangeCircle = null;
        }
    }

    private void MovePreviewToTile(Tile tile)
    {
        if (tile == null)
        {
            return;
        }

        MovePreviewToWorldPosition(tile.transform.position);
    }

    private void MovePreviewToWorldPosition(Vector3 worldPosition)
    {
        worldPosition.z = 0f;

        if (previewObject != null)
        {
            previewObject.transform.position = worldPosition;
        }

        if (previewRangeCircle != null)
        {
            previewRangeCircle.transform.position = worldPosition;
        }
    }

    private void SetPreviewValid(bool isValid)
    {
        Color color = isValid
            ? new Color(1f, 1f, 1f, 0.65f)
            : new Color(1f, 0.25f, 0.25f, 0.65f);

        if (previewSpriteRenderer != null)
        {
            previewSpriteRenderer.color = color;
        }
    }

    private bool CanAffordSelectedTower()
    {
        return GameManager.Instance != null &&
            selectedTowerTemplate != null &&
            GameManager.Instance.money >= selectedTowerTemplate.cost;
    }

    private bool IsTileValidForPlacement(Tile tile)
    {
        return tile != null && tile.isBuildable && !tile.isOccupied;
    }

    private Tile GetTileAtScreenPosition(Vector2 screenPosition)
    {
        return FindTile(Physics2D.OverlapPointAll(GetWorldPosition(screenPosition)));
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogError("Main camera is missing.");
            return Vector3.zero;
        }

        Vector3 worldPosition = cam.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    private Tile FindTile(Collider2D[] hits)
    {
        foreach (Collider2D hit in hits)
        {
            Tile tile = hit.GetComponent<Tile>();

            if (tile != null)
            {
                return tile;
            }
        }

        return null;
    }
}
