using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class InputHelper
{
    public static bool TryGetTapBegan(out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;

        if (IsPointerOverUI())
        {
            return false;
        }

        if (!IsTapStarted())
        {
            return false;
        }

        return TryGetPointerScreenPosition(out screenPosition);
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
        if (EventSystem.current == null)
        {
            return false;
        }

        return EventSystem.current.IsPointerOverGameObject();
    }
}
