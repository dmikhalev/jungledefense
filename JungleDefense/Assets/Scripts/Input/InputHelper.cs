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

        screenPosition = GetTapScreenPosition();
        return true;
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

    private static Vector2 GetTapScreenPosition()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
