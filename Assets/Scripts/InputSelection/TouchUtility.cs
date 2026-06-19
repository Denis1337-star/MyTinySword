using UnityEngine;
using UnityEngine.EventSystems;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Утилита для touch input
/// </summary>
public static class TouchUtility
{
    private const float MaxTapMovement = 25f;
    private const float MaxTapMovementSqr = MaxTapMovement * MaxTapMovement;

    /// <summary>
    /// Пытается получить завершённый короткий touch tap
    /// </summary>
    public static bool TryGetEndedTap(out Touch touch)
    {
        touch = default;

        var activeTouches = Touch.activeTouches;

        for (int i = 0; i < activeTouches.Count; i++)
        {
            Touch activeTouch = activeTouches[i];

            if (activeTouch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
                continue;

            Vector2 movement = activeTouch.screenPosition - activeTouch.startScreenPosition;

            if (movement.sqrMagnitude > MaxTapMovementSqr)
                continue;

            touch = activeTouch;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Проверяет, находится ли touch над UI
    /// </summary>
    public static bool IsPointerOverUI(Touch touch)
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject(touch.touchId);
    }

    /// <summary>
    /// Переводит экранную позицию touch в позицию мира
    /// </summary>
    public static Vector2 ScreenToWorld(Camera camera, Vector2 screenPosition)
    {
        if (camera == null)
            return Vector2.zero;

        return camera.ScreenToWorldPoint(screenPosition);
    }
}