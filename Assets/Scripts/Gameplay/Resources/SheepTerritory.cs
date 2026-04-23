using UnityEngine;

/// <summary>
/// Задаёт прямоугольную территорию, внутри которой может перемещаться овца
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class SheepTerritory : MonoBehaviour
{
    private BoxCollider2D box;
    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Возвращает случайную точку внутри границ территории
    /// </summary>
    public Vector2 GetRandomPoint()
    {
        var bounds = box.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }

    private void OnDrawGizmos()
    {
        if (box == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
    }
}

