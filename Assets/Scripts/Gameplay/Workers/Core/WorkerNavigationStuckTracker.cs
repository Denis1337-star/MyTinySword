using UnityEngine;

/// <summary>
/// Детектор залипания NavMesh: есть цель, но нет прогресса по позиции/скорости.
/// </summary>
public sealed class WorkerNavigationStuckTracker
{
    private const float StuckTimeoutSeconds = 1.75f;
    private const float MinProgressDistanceSqr = 0.04f;

    private float _stuckTimer;
    private Vector2 _lastPosition;
    private bool _initialized;

    public void Reset(Vector2 position)
    {
        _stuckTimer = 0f;
        _lastPosition = position;
        _initialized = true;
    }

    /// <summary>
    /// true — пора делать repath / сброс задания.
    /// </summary>
    public bool Tick(UnitMovement movement, Vector2 currentPosition, float deltaTime)
    {
        if (!_initialized)
            Reset(currentPosition);

        if (movement == null)
            return true;

        if (movement.HasFailedPath)
            return true;

        if (!movement.HasTarget)
        {
            _stuckTimer = 0f;
            _lastPosition = currentPosition;
            return false;
        }

        float movedSqr = (currentPosition - _lastPosition).sqrMagnitude;

        if (movedSqr >= MinProgressDistanceSqr || movement.IsMoving)
        {
            _lastPosition = currentPosition;
            _stuckTimer = 0f;
            return false;
        }

        _stuckTimer += deltaTime;
        return _stuckTimer >= StuckTimeoutSeconds;
    }
}
