using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public static class TouchUtility
{
    public static bool TryGetEndedTouch(out Touch touch)
    {
        if (Touch.activeTouches.Count == 0)
        {
            touch = default;
            return false;
        }

        touch = Touch.activeTouches[0];
        return touch.phase == TouchPhase.Ended;
    }

    public static bool IsPointerOverUI(Touch touch)
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject(touch.touchId);
    }

    public static Vector2 ScreenToWorld(Camera camera, Vector2 screenPosition)
    {
        if (camera == null)
            return Vector2.zero;

        return camera.ScreenToWorldPoint(screenPosition);
    }
}
