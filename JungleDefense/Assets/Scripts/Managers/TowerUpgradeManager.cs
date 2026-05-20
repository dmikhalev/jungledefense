using UnityEngine;

public class TowerUpgradeManager : MonoBehaviour
{
    public static TowerUpgradeManager Instance { get; private set; }

    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject deleteButton;

    private Camera mainCamera;
    private Tower selectedTower;

    public Tower SelectedTower => selectedTower;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;

        HideUpgradeButton();
        HideDeleteButton();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (TowerPlacementManager.Instance != null && TowerPlacementManager.Instance.IsBuildMode)
        {
            return;
        }

        if (InputHelper.TryGetTapBegan(out Vector2 screenPosition))
        {
            TrySelectTower(screenPosition);
        }
    }

    private void TrySelectTower(Vector2 screenPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            Tower tower = hit.GetComponent<Tower>();

            if (tower != null)
            {
                SelectTower(tower);
                return;
            }
        }

        ClearSelection();
    }

    private void SelectTower(Tower tower)
    {
        if (selectedTower != null && selectedTower != tower)
        {
            selectedTower.HideRange();
        }

        selectedTower = tower;
        selectedTower.ShowRange();

        if (upgradeButton != null)
        {
            upgradeButton.SetActive(!selectedTower.IsMaxLevel);
        }

        if (deleteButton != null)
        {
            deleteButton.SetActive(true);
        }

        Debug.Log($"Selected tower level: {selectedTower.Level}");
    }

    public void UpgradeSelectedTower()
    {
        if (selectedTower == null)
        {
            return;
        }

        bool upgraded = selectedTower.UpgradeTower();

        if (!upgraded)
        {
            return;
        }

        if (selectedTower.IsMaxLevel)
        {
            HideUpgradeButton();
        }
    }

    public void ClearSelection()
    {
        if (selectedTower != null)
        {
            selectedTower.HideRange();
        }

        selectedTower = null;
        HideUpgradeButton();
        HideDeleteButton();
    }

    private void HideUpgradeButton()
    {
        if (upgradeButton != null)
        {
            upgradeButton.SetActive(false);
        }
    }

    private void HideDeleteButton()
    {
        if (deleteButton != null)
        {
            deleteButton.SetActive(false);
        }
    }

    public void DeleteSelectedTower()
    {
        if (selectedTower == null)
        {
            return;
        }

        selectedTower.DeleteTower();
        selectedTower = null;

        HideUpgradeButton();
        HideDeleteButton();
    }
}
