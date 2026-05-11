using UnityEngine;

/// <summary>
/// »щет цели дл€ боевого юнита
/// </summary>
public sealed class ArmyTargetFinder
{
    private const int TargetBufferSize = 32;

    private readonly Collider2D[] _targetBuffer = new Collider2D[TargetBufferSize];

    private readonly ArmyUnit _unit;
    private readonly Transform _origin;

    public ArmyTargetFinder(ArmyUnit unit, Transform origin)
    {
        _unit = unit;
        _origin = origin;
    }

    public Health FindNearestEnemyTarget()
    {
        if (!CanSearch())
            return null;

        int hitCount = Physics2D.OverlapCircleNonAlloc( _origin.position,
            _unit.Config.VisionRange, _targetBuffer);

        if (hitCount == 0)
            return null;

        Health bestTarget = null;
        int bestPriority = int.MinValue;
        float bestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _targetBuffer[i];

            if (hit == null)
                continue;

            Health targetHealth = hit.GetComponentInParent<Health>();

            if (targetHealth == null || targetHealth.IsDead)
                continue;

            if (targetHealth == _unit.Health)
                continue;

            if (!IsEnemy(targetHealth))
                continue;

            CombatTargetInfo targetInfo = targetHealth.GetComponentInParent<CombatTargetInfo>();

            int priority = targetInfo != null
                ? (int)targetInfo.TargetPriority
                : (int)TargetPriorityType.ArmyUnit;

            float distanceSqr = (targetHealth.transform.position - _origin.position).sqrMagnitude;

            bool betterPriority = priority > bestPriority;
            bool samePriorityButCloser = priority == bestPriority && distanceSqr < bestDistanceSqr;

            if (betterPriority || samePriorityButCloser)
            {
                bestPriority = priority;
                bestDistanceSqr = distanceSqr;
                bestTarget = targetHealth;
            }
        }

        return bestTarget;
    }

    public Health FindLowestHealthAllyUnit()
    {
        if (!CanSearch())
            return null;

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            _origin.position,
            _unit.Config.VisionRange,
            _targetBuffer);

        if (hitCount == 0)
            return null;

        Health bestTarget = null;
        float lowestHealthPercent = 1f;
        float bestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _targetBuffer[i];

            if (hit == null)
                continue;

            Health targetHealth = hit.GetComponentInParent<Health>();

            if (targetHealth == null || targetHealth.IsDead)
                continue;

            if (targetHealth == _unit.Health)
                continue;

            if (IsEnemy(targetHealth))
                continue;

            if (targetHealth.CurrentHealth >= targetHealth.MaxHealth)
                continue;

            float healthPercent = targetHealth.MaxHealth > 0
                ? (float)targetHealth.CurrentHealth / targetHealth.MaxHealth
                : 1f;

            float distanceSqr = (targetHealth.transform.position - _origin.position).sqrMagnitude;

            bool lowerHealth = healthPercent < lowestHealthPercent;
            bool sameHealthButCloser = Mathf.Approximately(healthPercent, lowestHealthPercent) &&
                                       distanceSqr < bestDistanceSqr;

            if (lowerHealth || sameHealthButCloser)
            {
                lowestHealthPercent = healthPercent;
                bestDistanceSqr = distanceSqr;
                bestTarget = targetHealth;
            }
        }

        return bestTarget;
    }

    public bool IsEnemy(Health targetHealth)
    {
        if (targetHealth == null)
            return false;

        FactionMember targetFaction = targetHealth.GetComponentInParent<FactionMember>();

        if (targetFaction == null)
            return false;

        return _unit.FactionMember.IsEnemy(targetFaction);
    }

    private bool CanSearch()
    {
        return _unit != null &&
               !_unit.IsDead &&
               _unit.Config != null &&
               _unit.FactionMember != null;
    }
}