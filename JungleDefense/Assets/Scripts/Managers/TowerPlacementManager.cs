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
        // если башня не выбрана — ничего не делаем
        if (selectedTowerPrefab == null)
            return;

        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0;

        // округляем позицию (чтобы ровно ставилось)
        Vector3 buildPos = new Vector3(
            Mathf.Round(world.x),
            Mathf.Round(world.y),
            0
        );

        // проверка денег
        if (!GameManager.Instance.SpendMoney(selectedTowerCost))
        {
            Debug.Log("Недостаточно денег");
            return;
        }

        Instantiate(selectedTowerPrefab, buildPos, Quaternion.identity);
    }
}