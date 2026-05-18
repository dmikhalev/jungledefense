using UnityEngine;
using UnityEngine.EventSystems;

public class TowerUpgradeManager : MonoBehaviour
{
    public static TowerUpgradeManager Instance;

    public Tower selectedTower;
    public GameObject upgradeButton;

    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;

        if (upgradeButton != null)
        {
            upgradeButton.SetActive(false);
        }
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TrySelectTower(Input.mousePosition);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TrySelectTower(Input.GetTouch(0).position);
        }
    }

    void TrySelectTower(Vector2 screenPos)
    {
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        world.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(world);

        if (hit == null)
        {
            ClearSelection();
            return;
        }

        Tower tower = hit.GetComponent<Tower>();

        if (tower == null)
        {
            ClearSelection();
            return;
        }

        selectedTower = tower;

        if (upgradeButton != null)
        {
            upgradeButton.SetActive(true);
        }

        Debug.Log("Выбрана башня уровня: " + selectedTower.level);
    }

    public void UpgradeSelectedTower()
    {
        if (selectedTower == null)
        {
            return;
        }

        selectedTower.UpgradeTower();

        if (selectedTower.level >= selectedTower.maxLevel)
        {
            upgradeButton.SetActive(false);
        }
    }

    void ClearSelection()
    {
        selectedTower = null;

        if (upgradeButton != null)
        {
            upgradeButton.SetActive(false);
        }
    }
}