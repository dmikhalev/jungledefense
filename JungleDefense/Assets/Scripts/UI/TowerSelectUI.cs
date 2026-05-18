using UnityEngine;

public class TowerSelectUI : MonoBehaviour
{
    [Header("Tower prefabs")]
    [SerializeField] private GameObject monkeyTower;
    [SerializeField] private GameObject tigerTower;
    [SerializeField] private GameObject hippoTower;

    [Header("Tower costs")]
    [SerializeField] private int monkeyCost = 25;
    [SerializeField] private int tigerCost = 50;
    [SerializeField] private int hippoCost = 75;

    public void SelectMonkey()
    {
        SelectTower(monkeyTower, monkeyCost);
    }

    public void SelectTiger()
    {
        SelectTower(tigerTower, tigerCost);
    }

    public void SelectHippo()
    {
        SelectTower(hippoTower, hippoCost);
    }

    private void SelectTower(GameObject towerPrefab, int cost)
    {
        if (TowerPlacementManager.Instance == null)
        {
            Debug.LogError("TowerPlacementManager is missing from the scene.");
            return;
        }

        TowerPlacementManager.Instance.selectedTowerPrefab = towerPrefab;
        TowerPlacementManager.Instance.selectedTowerCost = cost;
    }
}
