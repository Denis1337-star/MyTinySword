using UnityEngine;

/// <summary>
/// ”ниверсальное описание gameplay указател€
/// </summary>
public readonly struct GameplayPointer
{
    public const int MousePointerId = -1;

    public Vector2 ScreenPosition { get; }
    public int PointerId { get; }
    public bool IsTouch { get; }

    public bool IsMouse => !IsTouch;

    private GameplayPointer(Vector2 screenPosition, int pointerId, bool isTouch)
    {
        ScreenPosition = screenPosition;
        PointerId = pointerId;
        IsTouch = isTouch;
    }

    public static GameplayPointer FromTouch(Vector2 screenPosition, int touchId)
    {
        return new GameplayPointer(screenPosition, touchId, true);
    }

    public static GameplayPointer FromMouse(Vector2 screenPosition)
    {
        return new GameplayPointer(screenPosition, MousePointerId, false);
    }
}