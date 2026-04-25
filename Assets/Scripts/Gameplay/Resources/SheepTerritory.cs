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
        if (box == null)
            box = GetComponent<BoxCollider2D>();
    }

    public Vector2 GetRandomPoint()
    {
        Bounds bounds = box.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }
}

