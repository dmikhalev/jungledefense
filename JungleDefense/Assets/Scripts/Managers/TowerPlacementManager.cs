using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }

    public GameObject selectedTowerPrefab;
    public int selectedTowerCost;

    public bool IsBuildMode => selectedTowerPrefab != null;

    private Camera cam;

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
            return;
        }

        if (InputHelper.TryGetTapBegan(out Vector2 screenPosition))
        {
            TryPlaceTower(screenPosition);
        }
    }

    public void SelectTowerForBuilding(GameObject towerPrefab, int cost)
    {
        selectedTowerPrefab = towerPrefab;
        selectedTowerCost = cost;
    }

    public void ClearSelection()
    {
        selectedTowerPrefab = null;
        selectedTowerCost = 0;
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

        if (!GameManager.Instance.SpendMoney(selectedTowerCost))
        {
            Debug.Log("Not enough money.");
            return;
        }

        Instantiate(selectedTowerPrefab, tile.transform.position, Quaternion.identity);
        tile.isOccupied = true;

        Debug.Log("Tower placed.");
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
