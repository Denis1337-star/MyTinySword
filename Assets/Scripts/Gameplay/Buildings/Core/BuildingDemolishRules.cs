using UnityEngine.UI;

/// <summary>
/// Правила сноса зданий через UI.
/// </summary>
public static class BuildingDemolishRules
{
    public static bool CanDemolish(BuildingBase building)
    {
        if (building == null)
            return false;

        if (!building.CanBeDemolishedByButton)
            return false;

        return TutorialInputGuard.AllowsDemolishBuilding();
    }

    public static void RefreshButton(Button demolishButton, BuildingBase building)
    {
        if (demolishButton == null)
            return;

        if (building == null || !building.CanBeDemolishedByButton)
        {
            demolishButton.gameObject.SetActive(false);
            return;
        }

        demolishButton.gameObject.SetActive(true);
        demolishButton.interactable = TutorialInputGuard.AllowsDemolishBuilding();
    }
}
