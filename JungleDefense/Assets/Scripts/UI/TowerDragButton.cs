using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDragButton : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject towerPrefab;

    private bool isDragging;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging)
        {
            return;
        }

        TowerPlacementManager.Instance.SelectTowerForBuilding(towerPrefab);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        TowerPlacementManager.Instance.BeginTowerDrag(towerPrefab);
    }

    public void OnDrag(PointerEventData eventData)
    {
        TowerPlacementManager.Instance.UpdateTowerDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (eventData.pointerEnter != null &&
            eventData.pointerEnter.GetComponentInParent<TowerDragButton>() != null)
        {
            TowerPlacementManager.Instance.ClearSelection();
            return;
        }

        TowerPlacementManager.Instance.EndTowerDrag(eventData.position);
    }
}