using UnityEngine;

public class TowerSelectUI : MonoBehaviour
{
    [Header("Tower prefabs")]
    [SerializeField] private GameObject monkeyTower;
    [SerializeField] private GameObject tigerTower;
    [SerializeField] private GameObject hippoTower;

    public void SelectMonkey()
    {
        SelectTower(monkeyTower);
    }

    public void SelectTiger()
    {
        SelectTower(tigerTower);
    }

    public void SelectHippo()
    {
        SelectTower(hippoTower);
    }

    private void SelectTower(GameObject towerPrefab)
    {
        if (towerPrefab == null)
        {
            Debug.LogError("Tower prefab is not assigned.");
            return;
        }

        if (TowerPlacementManager.Instance == null)
        {
            Debug.LogError("TowerPlacementManager is missing from the scene.");
            return;
        }

        TowerPlacementManager.Instance.SelectTowerForBuilding(towerPrefab);
    }
}
