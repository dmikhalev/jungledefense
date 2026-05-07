using UnityEngine;
using UnityEngine.EventSystems;

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
        // Не ставим башни при нажатии на UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Mouse (Editor)
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower(Input.mousePosition);
        }

        // Touch (iPhone)
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryPlaceTower(Input.GetTouch(0).position);
        }
    }

    void TryPlaceTower(Vector2 screenPos)
    {
        if (selectedTowerPrefab == null)
        {
            return;
        }

        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(world);

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
            Debug.Log("Нельзя строить на этой клетке");
            return;
        }

        if (tile.isOccupied)
        {
            Debug.Log("Клетка уже занята");
            return;
        }

        if (!GameManager.Instance.SpendMoney(selectedTowerCost))
        {
            Debug.Log("Недостаточно денег");
            return;
        }

        Instantiate(
            selectedTowerPrefab,
            hit.transform.position,
            Quaternion.identity
        );

        tile.isOccupied = true;
    }
}