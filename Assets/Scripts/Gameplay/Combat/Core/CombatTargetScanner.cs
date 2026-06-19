using System;
using UnityEngine;

/// <summary>
/// Общий overlap-поиск целей в радиусе с выбором лучшей по приоритету и дистанции.
/// </summary>
public static class CombatTargetScanner
{
    public static Health FindBestTarget(
        Vector2 origin,
        float range,
        Collider2D[] buffer,
        Func<Collider2D, Health, bool> isValidTarget,
        Func<Collider2D, int> getPriority)
    {
        if (buffer == null || isValidTarget == null || getPriority == null)
            return null;

        int hitCount = Physics2D.OverlapCircleNonAlloc(origin, range, buffer);

        if (hitCount == 0)
            return null;

        Health bestTarget = null;
        int bestPriority = int.MinValue;
        float bestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = buffer[i];

            if (hit == null)
                continue;

            if (!hit.TryGetComponent(out Health targetHealth))
                continue;

            if (!isValidTarget(hit, targetHealth))
                continue;

            int priority = getPriority(hit);
            float distanceSqr = ((Vector2)targetHealth.transform.position - origin).sqrMagnitude;

            if (!IsBetterTarget(priority, distanceSqr, bestPriority, bestDistanceSqr))
                continue;

            bestPriority = priority;
            bestDistanceSqr = distanceSqr;
            bestTarget = targetHealth;
        }

        return bestTarget;
    }

    private static bool IsBetterTarget(
        int priority,
        float distanceSqr,
        int bestPriority,
        float bestDistanceSqr)
    {
        if (priority > bestPriority)
            return true;

        if (priority < bestPriority)
            return false;

        return distanceSqr < bestDistanceSqr;
    }
}
