using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelectUI : MonoBehaviour
{
    private const float DragClickSuppressThreshold = 25f;

    [Header("Tower prefabs")]
    [SerializeField] private GameObject monkeyTower;
    [SerializeField] private GameObject tigerTower;
    [SerializeField] private GameObject hippoTower;

    private Vector2 dragStartPosition;
    private bool suppressNextClick;

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
        BeginDrag(monkeyTower, eventData);
    }

    public void BeginDragTiger(BaseEventData eventData)
    {
        BeginDrag(tigerTower, eventData);
    }

    public void BeginDragHippo(BaseEventData eventData)
    {
        BeginDrag(hippoTower, eventData);
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

        float dragDistance = Vector2.Distance(dragStartPosition, pointerData.position);
        suppressNextClick = dragDistance >= DragClickSuppressThreshold;

        TowerPlacementManager.Instance.EndTowerDrag(pointerData.position);
    }

    private void SelectTower(GameObject towerPrefab)
    {
        if (suppressNextClick)
        {
            suppressNextClick = false;
            return;
        }

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

    private void BeginDrag(GameObject towerPrefab, BaseEventData eventData)
    {
        suppressNextClick = false;

        if (eventData is PointerEventData pointerData)
        {
            dragStartPosition = pointerData.position;
        }

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
