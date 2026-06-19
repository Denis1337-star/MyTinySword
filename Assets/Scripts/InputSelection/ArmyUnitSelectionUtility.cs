using System.Collections.Generic;

/// <summary>
/// Проверки выбранных боевых юнитов игрока.
/// </summary>
public static class ArmyUnitSelectionUtility
{
    public static bool TryGetPlayerArmyUnit(
        UnitSelectable selectable,
        out ArmyUnit armyUnit,
        bool includeDead = false)
    {
        armyUnit = null;

        if (selectable == null)
            return false;

        armyUnit = SelectableUtility.FindNear<ArmyUnit>(selectable);

        if (armyUnit == null)
            return false;

        if (!armyUnit.IsPlayerUnit())
            return false;

        if (!includeDead && armyUnit.IsDead)
            return false;

        return true;
    }

    public static bool HasAnyPlayerArmyUnit(
        IReadOnlyList<UnitSelectable> selectedUnits,
        bool includeDead = false)
    {
        if (selectedUnits == null || selectedUnits.Count == 0)
            return false;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (TryGetPlayerArmyUnit(selectedUnits[i], out ArmyUnit _, includeDead))
                return true;
        }

        return false;
    }
}
