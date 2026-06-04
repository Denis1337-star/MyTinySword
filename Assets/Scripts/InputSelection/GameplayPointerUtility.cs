using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Общая утилита для gameplay pointer input
/// </summary>
public static class GameplayPointerUtility
{
    private const float MaxMouseClickMovement = 25f;
    private const float MaxMouseClickMovementSqr = MaxMouseClickMovement * MaxMouseClickMovement;

    private static Vector2 _mousePressScreenPosition;
    private static bool _hasMousePress;
    private static bool _mousePressStartedOverUi;


    public static bool TryGetEndedTap(out GameplayPointer pointer)
    {
        if (TryGetTouchTap(out pointer))
            return true;

        if (TryGetMouseClick(out pointer))
            return true;

        pointer = default;
        return false;
    }


    public static bool IsPointerOverUI(GameplayPointer pointer)
    {
        if (EventSystem.current == null)
            return false;

        if (pointer.IsTouch)
            return EventSystem.current.IsPointerOverGameObject(pointer.PointerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    private static bool TryGetTouchTap(out GameplayPointer pointer)
    {
        pointer = default;

        if (!TouchUtility.TryGetEndedTap(out Touch touch))
            return false;

        pointer = GameplayPointer.FromTouch(touch.screenPosition, touch.touchId);
        return true;
    }

    private static bool TryGetMouseClick(out GameplayPointer pointer)
    {
        pointer = default;

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return false;

        Vector2 currentScreenPosition = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame)
        {
            _mousePressScreenPosition = currentScreenPosition;
            _hasMousePress = true;
            _mousePressStartedOverUi = IsMouseOverUI();
        }

        if (!mouse.leftButton.wasReleasedThisFrame)
            return false;

        if (!_hasMousePress)
        {
            ResetMousePressState();
            return false;
        }

        bool startedOverUi = _mousePressStartedOverUi;

        Vector2 movement = currentScreenPosition - _mousePressScreenPosition;

        ResetMousePressState();

        if (startedOverUi)
            return false;

        if (movement.sqrMagnitude > MaxMouseClickMovementSqr)
            return false;

        pointer = GameplayPointer.FromMouse(currentScreenPosition);
        return true;
    }

    private static bool IsMouseOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private static void ResetMousePressState()
    {
        _hasMousePress = false;
        _mousePressStartedOverUi = false;
        _mousePressScreenPosition = Vector2.zero;
    }
}