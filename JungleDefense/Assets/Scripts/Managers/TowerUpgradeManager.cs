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
    [SerializeField] private UnityEngine.UI.Image towerIcon;

    [Header("Buttons")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;

    private const float SelectionRadius = 0.22f;
    private readonly Collider2D[] selectionHits = new Collider2D[16];

    private Tower selectedTower;
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

        HideUI();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsGameplayActive)
        {
            return;
        }

        if (towerInfoPanel != null &&
            towerInfoPanel.activeSelf &&
            selectedTower != null)
        {
            RefreshUI();
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
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                return;
            }
        }

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Tower tower = FindTowerAt(worldPosition);

        if (tower == null)
        {
            HideUI();
            return;
        }

        if (selectedTower != null && selectedTower != tower)
        {
            selectedTower.HideRange();
        }

        selectedTower = tower;
        selectedTower.ShowRange();

        ShowUI();
    }

    private Tower FindTowerAt(Vector2 worldPosition)
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            worldPosition,
            SelectionRadius,
            selectionHits
        );

        Tower closestTower = null;
        float closestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = selectionHits[i];
            selectionHits[i] = null;

            if (hit == null)
            {
                continue;
            }

            Tower tower = hit.GetComponent<Tower>();

            if (tower == null)
            {
                tower = hit.GetComponentInParent<Tower>();
            }

            if (tower == null)
            {
                continue;
            }

            float distanceSqr = ((Vector2)tower.transform.position - worldPosition).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestTower = tower;
            }
        }

        return closestTower;
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

        if (towerIcon != null)
        {
            towerIcon.sprite = selectedTower.Icon;
            towerIcon.enabled = selectedTower.Icon != null;
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

        if (towerIcon != null)
        {
            towerIcon.enabled = false;
        }
    }
}