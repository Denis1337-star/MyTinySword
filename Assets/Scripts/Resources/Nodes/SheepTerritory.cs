using UnityEngine;

/// <summary>
/// Задаёт прямоугольную территорию внутри которой может перемещаться овца
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public sealed class SheepTerritory : MonoBehaviour
{
    private BoxCollider2D _box;

    private void Awake()
    {
        _box = GetComponent<BoxCollider2D>();
    }

    public Vector2 GetRandomPoint()
    {
        Bounds bounds = _box.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }
}