using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    public int cost = 50; // Стоимость башни
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 gridPosition = new Vector3(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y), 0);

            // Проверка, есть ли достаточно очков
            if (GameManager.Instance.money >= cost)
            {
                Instantiate(towerPrefab, gridPosition, Quaternion.identity);
                GameManager.Instance.AddMoney(-cost); // Уменьшаем очки
            }
        }
    }
}