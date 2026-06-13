using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerDragButton : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static readonly List<TowerDragButton> Buttons = new();

    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private float dimmedAlpha = 0.45f;

    private bool isDragging;
    private bool isSubscribed;
    private Image rootImage;
    private Image iconImage;
    private Color defaultIconColor;

    private void Awake()
    {
        rootImage = GetComponent<Image>();
        iconImage = FindIconImage();

        if (rootImage != null)
        {
            Color transparent = rootImage.color;
            transparent.a = 0f;
            rootImage.color = transparent;
        }

        if (iconImage != null)
        {
            defaultIconColor = iconImage.color;
        }

        SetIconAlpha(1f);
    }

    private void OnEnable()
    {
        if (!Buttons.Contains(this))
        {
            Buttons.Add(this);
        }

        TrySubscribeToPlacementManager();
    }

    private void Start()
    {
        // TowerPlacementManager.Instance can be created after this button's OnEnable.
        TrySubscribeToPlacementManager();
        RefreshFromCurrentSelection();
    }

    private void OnDisable()
    {
        Buttons.Remove(this);
        UnsubscribeFromPlacementManager();
        SetIconAlpha(1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging)
        {
            return;
        }

        if (TowerPlacementManager.Instance == null)
        {
            return;
        }

        TrySubscribeToPlacementManager();

        TowerPlacementManager.Instance.SelectTowerForBuilding(towerPrefab);
        RefreshFromCurrentSelection();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        if (TowerPlacementManager.Instance != null)
        {
            TrySubscribeToPlacementManager();
            TowerPlacementManager.Instance.BeginTowerDrag(towerPrefab);
            RefreshFromCurrentSelection();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (TowerPlacementManager.Instance != null)
        {
            TowerPlacementManager.Instance.UpdateTowerDrag(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (TowerPlacementManager.Instance == null)
        {
            return;
        }

        if (eventData.pointerEnter != null &&
            eventData.pointerEnter.GetComponentInParent<TowerDragButton>() != null)
        {
            TowerPlacementManager.Instance.ClearSelection();
            RefreshFromCurrentSelection();
            return;
        }

        TowerPlacementManager.Instance.EndTowerDrag(eventData.position);
        RefreshFromCurrentSelection();
    }

    private void TrySubscribeToPlacementManager()
    {
        if (isSubscribed || TowerPlacementManager.Instance == null)
        {
            return;
        }

        TowerPlacementManager.Instance.SelectionChanged += OnTowerSelectionChanged;
        isSubscribed = true;
    }

    private void UnsubscribeFromPlacementManager()
    {
        if (!isSubscribed || TowerPlacementManager.Instance == null)
        {
            isSubscribed = false;
            return;
        }

        TowerPlacementManager.Instance.SelectionChanged -= OnTowerSelectionChanged;
        isSubscribed = false;
    }

    private void OnTowerSelectionChanged(GameObject selectedTowerPrefab)
    {
        RefreshAllButtons(selectedTowerPrefab);
    }

    private static void RefreshFromCurrentSelection()
    {
        GameObject selectedTowerPrefab = TowerPlacementManager.Instance != null
            ? TowerPlacementManager.Instance.selectedTowerPrefab
            : null;

        RefreshAllButtons(selectedTowerPrefab);
    }

    private static void RefreshAllButtons(GameObject selectedTowerPrefab)
    {
        bool hasSelection = selectedTowerPrefab != null;

        foreach (TowerDragButton button in Buttons)
        {
            if (button == null)
            {
                continue;
            }

            bool isSelectedButton = hasSelection && button.towerPrefab == selectedTowerPrefab;
            bool shouldDim = hasSelection && !isSelectedButton;

            button.SetIconAlpha(shouldDim ? button.dimmedAlpha : 1f);
        }
    }

    private void SetIconAlpha(float alpha)
    {
        if (iconImage == null)
        {
            return;
        }

        Color color = defaultIconColor;
        color.a = alpha;
        iconImage.color = color;
    }

    private Image FindIconImage()
    {
        Transform iconTransform = transform.Find("Icon");

        if (iconTransform != null &&
            iconTransform.TryGetComponent(out Image image))
        {
            return image;
        }

        Image[] images = GetComponentsInChildren<Image>(true);

        foreach (Image img in images)
        {
            if (img != null && img.gameObject != gameObject)
            {
                return img;
            }
        }

        return null;
    }
}
