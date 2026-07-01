using UnityEngine;

/// <summary>
/// Ищет цели для боевого юнита
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

        return CombatTargetScanner.FindBestTarget(
            _origin.position,
            _unit.Config.VisionRange,
            _targetBuffer,
            IsValidEnemyTarget,
            GetEnemyTargetPriority);
    }

    private bool IsValidEnemyTarget(Collider2D hit, Health targetHealth)
    {
        if (targetHealth == null || targetHealth.IsDead)
            return false;

        if (targetHealth == _unit.Health)
            return false;

        return IsEnemy(targetHealth);
    }

    private int GetEnemyTargetPriority(Collider2D hit)
    {
        return CombatTargetPriorityResolver.Resolve(hit);
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

            Health targetHealth = hit.GetComponent<Health>();

            if (targetHealth == null)
                continue;

            if (!CanHealAllyUnit(targetHealth))
                continue;

            float healthPercent = targetHealth.HealthPercent;
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

        FactionType? targetFaction = FactionResolver.TryGetFaction(targetHealth);

        if (targetFaction == null)
            return false;

        return FactionRules.IsEnemy(_unit.Faction, targetFaction.Value);
    }

    private bool CanHealAllyUnit(Health targetHealth)
    {
        if (targetHealth == null)
            return false;

        if (!targetHealth.CanBeHealed)
            return false;

        if (targetHealth == _unit.Health)
            return false;

        if (IsEnemy(targetHealth))
            return false;

        ArmyUnit targetUnit = targetHealth.GetComponent<ArmyUnit>();

        if (targetUnit == null || targetUnit.IsDead)
            return false;

        return true;
    }

    private bool CanSearch()
    {
        return _unit != null &&
               !_unit.IsDead &&
               _unit.Config != null;
    }
}