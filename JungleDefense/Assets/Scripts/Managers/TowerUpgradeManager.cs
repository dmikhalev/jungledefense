using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeManager : MonoBehaviour
{
    public static TowerUpgradeManager Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject towerInfoPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Buttons")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;

    private Tower selectedTower;
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;

        HideUI();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (TowerPlacementManager.Instance != null &&
            TowerPlacementManager.Instance.IsBuildMode)
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

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null)
        {
            HideUI();
            return;
        }

        Tower tower = hit.GetComponent<Tower>();

        if (tower == null)
        {
            HideUI();
            return;
        }

        if (selectedTower != null)
        {
            selectedTower.HideRange();
        }

        selectedTower = tower;
        selectedTower.ShowRange();

        ShowUI();
    }

    private void ShowUI()
    {
        if (selectedTower == null)
        {
            HideUI();
            return;
        }

        if (towerInfoPanel != null)
        {
            towerInfoPanel.SetActive(true);
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (selectedTower == null)
        {
            HideUI();
            return;
        }

        if (titleText != null)
        {
            titleText.text = selectedTower.GetTitleText();
        }

        if (statsText != null)
        {
            statsText.text = selectedTower.GetStatsText();
        }

        if (upgradeButton != null)
        {
            upgradeButton.interactable = selectedTower.CanUpgrade();
        }

        if (sellButton != null)
        {
            sellButton.interactable = true;
        }
    }

    public void UpgradeSelectedTower()
    {
        if (selectedTower == null)
        {
            return;
        }

        selectedTower.UpgradeTower();
        RefreshUI();
    }

    public void DeleteSelectedTower()
    {
        if (selectedTower == null)
        {
            return;
        }

        selectedTower.DeleteTower();
        HideUI();
    }

    public void HideUI()
    {
        if (selectedTower != null)
        {
            selectedTower.HideRange();
        }

        selectedTower = null;

        if (towerInfoPanel != null)
        {
            towerInfoPanel.SetActive(false);
        }
    }
}