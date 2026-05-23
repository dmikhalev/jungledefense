using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class InputHelper
{
    private static readonly List<RaycastResult> UiRaycastResults = new();

    public static bool TryGetTapBegan(out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;

        if (!IsTapStarted())
        {
            return false;
        }

        if (!TryGetPointerScreenPosition(out screenPosition))
        {
            return false;
        }

        return !IsScreenPositionOverUI(screenPosition);
    }

    public static bool TryGetPointerScreenPosition(out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        if (Mouse.current != null)
        {
            screenPosition = Mouse.current.position.ReadValue();
            return true;
        }

        return false;
    }

    private static bool IsTapStarted()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            return true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    public static bool IsPointerOverUI()
    {
        return TryGetPointerScreenPosition(out Vector2 screenPosition) &&
            IsScreenPositionOverUI(screenPosition);
    }

    public static bool IsScreenPositionOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        PointerEventData pointerData = new(EventSystem.current)
        {
            position = screenPosition
        };

        UiRaycastResults.Clear();
        EventSystem.current.RaycastAll(pointerData, UiRaycastResults);

        return UiRaycastResults.Count > 0;
    }
}
