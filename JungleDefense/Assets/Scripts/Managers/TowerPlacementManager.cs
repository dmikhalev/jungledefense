using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance;

    public GameObject selectedTowerPrefab;
    public int selectedTowerCost;

    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    void Update()
    {
        // ПК (мышка)
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower(Input.mousePosition);
        }

        // Мобилка (тач)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryPlaceTower(Input.GetTouch(0).position);
        }
    }

    void TryPlaceTower(Vector2 screenPos)
    {
        if (selectedTowerPrefab == null)
            return;

        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(world);

        if (hit == null)
            return;

        Tile tile = hit.GetComponent<Tile>();

        if (tile == null)
            return;

        if (!tile.isBuildable)
        {
            Debug.Log("Нельзя строить здесь");
            return;
        }

        if (tile.isOccupied)
        {
            Debug.Log("Клетка занята");
            return;
        }

        if (!GameManager.Instance.SpendMoney(selectedTowerCost))
            return;

        Instantiate(selectedTowerPrefab, hit.transform.position, Quaternion.identity);

        tile.isOccupied = true;
    }
}