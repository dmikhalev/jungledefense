using UnityEngine;
using UnityEngine.EventSystems;

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

    public void BeginDragMonkey(BaseEventData eventData)
    {
        BeginDrag(monkeyTower);
    }

    public void BeginDragTiger(BaseEventData eventData)
    {
        BeginDrag(tigerTower);
    }

    public void BeginDragHippo(BaseEventData eventData)
    {
        BeginDrag(hippoTower);
    }

    public void EndDrag(BaseEventData eventData)
    {
        if (TowerPlacementManager.Instance == null)
        {
            return;
        }

        PointerEventData pointerData = eventData as PointerEventData;

        if (pointerData == null)
        {
            return;
        }

        TowerPlacementManager.Instance.EndTowerDrag(pointerData.position);
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

    private void BeginDrag(GameObject towerPrefab)
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

        TowerPlacementManager.Instance.BeginTowerDrag(towerPrefab);
    }
}
