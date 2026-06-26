using UnityEngine;

/// <summary>
/// Задаёт прямоугольную территорию внутри которой может перемещаться овца
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public sealed class SheepTerritory : ValidatedMonoBehaviour
{
    [SerializeField] private BoxCollider2D _box;

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _box, nameof(_box));
    }

    public Vector2 GetRandomPoint()
    {
        Bounds bounds = _box.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }
}
