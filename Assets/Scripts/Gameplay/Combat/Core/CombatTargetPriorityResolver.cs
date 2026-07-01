using UnityEngine;

public static class CombatTargetPriorityResolver
{
    public static int Resolve(Collider2D hit)
    {
        if (hit == null)
            return (int)TargetPriorityType.ArmyUnit;

        ArmyUnit armyUnit = hit.GetComponent<ArmyUnit>();
        if (armyUnit != null)
            return (int)armyUnit.TargetPriority;

        BuildingBase building = hit.GetComponent<BuildingBase>();
        if (building != null)
            return (int)building.TargetPriority;

        Worker worker = hit.GetComponent<Worker>();
        if (worker != null)
            return (int)TargetPriorityType.Worker;

        return (int)TargetPriorityType.Building;
    }
}