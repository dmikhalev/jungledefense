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

        UpdatePreview();

        if (InputHelper.TryGetTapBegan(out Vector2 screenPosition))
        {
            TryPlaceTower(screenPosition);
        }
    }

    public void SelectTowerForBuilding(GameObject towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;
        selectedTowerTemplate = towerPrefab != null ? towerPrefab.GetComponent<Tower>() : null;
        CreatePreview();
    }

    public void ClearSelection()
    {
        selectedTowerPrefab = null;
        selectedTowerTemplate = null;

        DestroyPreview();
    }

    private void TryPlaceTower(Vector2 screenPosition)
    {
        Vector3 worldPosition = cam.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);
        Tile tile = FindTile(hits);

        if (tile == null)
        {
            Debug.Log("No tile hit.");
            return;
        }

        if (!tile.isBuildable)
        {
            Debug.Log("Tile is not buildable.");
            return;
        }

        if (tile.isOccupied)
        {
            Debug.Log("Tile is occupied.");
            return;
        }

        if (selectedTowerTemplate == null)
        {
            Debug.LogError("Selected tower prefab has no Tower component.");
            ClearSelection();
            return;
        }

        if (!GameManager.Instance.SpendMoney(selectedTowerTemplate.cost))
        {
            Debug.Log("Not enough money.");
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

        Debug.Log("Tower placed.");
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

        if (!InputHelper.TryGetPointerScreenPosition(out Vector2 screenPosition))
        {
            return;
        }

        Vector3 worldPosition = cam.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Tile tile = FindTile(Physics2D.OverlapPointAll(worldPosition));

        if (tile != null)
        {
            previewObject.transform.position = tile.transform.position;
            SetPreviewValid(tile.isBuildable && !tile.isOccupied);
        }
        else
        {
            previewObject.transform.position = worldPosition;
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

        SpriteRenderer sourceSprite = selectedTowerPrefab.GetComponent<SpriteRenderer>();

        if (sourceSprite != null)
        {
            previewSpriteRenderer = previewObject.AddComponent<SpriteRenderer>();
            previewSpriteRenderer.sprite = sourceSprite.sprite;
            previewSpriteRenderer.sortingLayerID = sourceSprite.sortingLayerID;
            previewSpriteRenderer.sortingOrder = sourceSprite.sortingOrder + 100;
        }

        previewRangeCircle = previewObject.AddComponent<RangeCircleRenderer>();

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
            previewRangeCircle = null;
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
