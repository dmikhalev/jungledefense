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
        Instance = this;
        cam = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (selectedTowerPrefab == null)
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
        Debug.Log("Try place tower");

        Vector3 worldPosition = cam.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null)
        {
            Debug.Log("No tile hit");
            return;
        }

        Tile tile = hit.GetComponent<Tile>();

        if (tile == null)
        {
            Debug.Log("Hit object is not Tile: " + hit.name);
            return;
        }

        if (!tile.isBuildable)
        {
            Debug.Log("Tile is not buildable");
            return;
        }

        if (tile.isOccupied)
        {
            Debug.Log("Tile is occupied");
            return;
        }

        if (!GameManager.Instance.SpendMoney(selectedTowerCost))
        {
            Debug.Log("Not enough money");
            return;
        }

        Instantiate(selectedTowerPrefab, hit.transform.position, Quaternion.identity);
        tile.isOccupied = true;

        Debug.Log("Tower placed");
    }
}
