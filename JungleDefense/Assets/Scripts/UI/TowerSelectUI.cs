using UnityEngine;

public class TowerSelectUI : MonoBehaviour
{
    public GameObject monkeyTower;
    public GameObject tigerTower;
    public GameObject hippoTower;

    public int monkeyCost = 25;
    public int tigerCost = 50;
    public int hippoCost = 75;

    public void SelectMonkey()
    {
        Debug.Log("Выбрана обезьяна");

        TowerPlacementManager.Instance.selectedTowerPrefab = monkeyTower;
        TowerPlacementManager.Instance.selectedTowerCost = monkeyCost;
    }

    public void SelectTiger()
    {
        Debug.Log("Выбран тигр");

        TowerPlacementManager.Instance.selectedTowerPrefab = tigerTower;
        TowerPlacementManager.Instance.selectedTowerCost = tigerCost;
    }

    public void SelectHippo()
    {
        Debug.Log("Выбран бегемот");

        TowerPlacementManager.Instance.selectedTowerPrefab = hippoTower;
        TowerPlacementManager.Instance.selectedTowerCost = hippoCost;
    }
}