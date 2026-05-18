using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }

    [Header("Selected tower")]
    public GameObject selectedTowerPrefab;
    public int selectedTowerCost;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (InputHelper.IsPointerOverUI())
        {
            return;
        }

        if (InputHelper.TryGetTapBegan(out Vector2 screenPosition))
        {
            TryPlaceTower(screenPosition);
        }
    }

    private void TryPlaceTower(Vector2 screenPosition)
    {
        if (selectedTowerPrefab == null)
        {
            return;
        }

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null)
        {
            return;
        }

        Tile tile = hit.GetComponent<Tile>();

        if (tile == null)
        {
            return;
        }

        if (!tile.isBuildable)
        {
            Debug.Log("Cannot build on this tile.");
            return;
        }

        if (tile.isOccupied)
        {
            Debug.Log("Tile is already occupied.");
            return;
        }

        if (!GameManager.Instance.SpendMoney(selectedTowerCost))
        {
            Debug.Log("Not enough money.");
            return;
        }

        Instantiate(selectedTowerPrefab, hit.transform.position, Quaternion.identity);
        tile.isOccupied = true;
    }
}
