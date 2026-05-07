using UnityEngine;
using UnityEngine.InputSystem;

public static class InputHelper
{
    public static bool IsTap()
    {
        // Touchscreen (iPhone)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            return true;
        }

        // Mouse (Editor)
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    public static Vector2 GetTapPosition()
    {
        // Touchscreen
        if (Touchscreen.current != null)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        // Mouse
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }
}