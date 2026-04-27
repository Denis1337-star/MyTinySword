using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Единая утилита для touch-ввода
/// Используется gameplay-системами для обработки tap-действий
/// </summary>
public static class TouchUtility
{

    private const float MaxTapMovement = 20f;

    /// <summary>
    /// Пытается получить завершённый tap
    /// </summary>
    public static bool TryGetEndedTap(out Touch touch)
    {
        touch = default;

        foreach (Touch activeTouch in Touch.activeTouches)
        {
            if (activeTouch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
                continue;

            float movedDistance = Vector2.Distance(
                activeTouch.startScreenPosition,
                activeTouch.screenPosition);

            if (movedDistance > MaxTapMovement)
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
    /// Переводит экранную позицию в позицию мира
    /// </summary>
    public static Vector2 ScreenToWorld(Camera camera, Vector2 screenPosition)
    {
        if (camera == null)
            return Vector2.zero;

        return camera.ScreenToWorldPoint(screenPosition);
    }
}
