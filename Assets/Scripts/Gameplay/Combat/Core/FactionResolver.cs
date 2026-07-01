using UnityEngine;

public static class FactionResolver
{
    public static FactionType? TryGetFaction(Component component)
    {
        if (component == null)
            return null;

        ArmyUnit armyUnit = component.GetComponent<ArmyUnit>();
        if (armyUnit != null)
            return armyUnit.Faction;

        BuildingBase building = component.GetComponent<BuildingBase>();
        if (building != null)
            return building.Faction;

        Worker worker = component.GetComponent<Worker>();
        if (worker != null)
            return worker.Faction;

        return null;
    }

    public static FactionType? TryGetFaction(Health health)
    {
        if (health == null)
            return null;

        return TryGetFaction((Component)health);
    }
}