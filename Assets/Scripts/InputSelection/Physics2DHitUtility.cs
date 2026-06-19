using UnityEngine;

/// <summary>
/// Общий raycast по точке экрана / миру для gameplay-систем.
/// </summary>
public static class Physics2DHitUtility
{
    public static int OverlapAtScreen(
        Camera camera,
        Vector2 screenPosition,
        Collider2D[] buffer,
        LayerMask layerMask = default)
    {
        if (camera == null || buffer == null)
            return 0;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(camera, screenPosition);

        return OverlapAtWorld(worldPosition, buffer, layerMask);
    }

    public static int OverlapAtWorld(
        Vector2 worldPosition,
        Collider2D[] buffer,
        LayerMask layerMask = default)
    {
        if (buffer == null)
            return 0;

        if (layerMask.value == 0)
            return Physics2D.OverlapPointNonAlloc(worldPosition, buffer);

        return Physics2D.OverlapPointNonAlloc(worldPosition, buffer, layerMask);
    }
}
