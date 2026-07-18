/// <summary>
/// Обратная совместимость: TechTree-панели раньше читали TechTreeUiText.
/// Источник правды теперь — GameUiText.
/// </summary>
public static class TechTreeUiText
{
    public static string Upgrade => GameUiText.Upgrade;
    public static string Upgrading => GameUiText.Upgrading;
    public static string AlreadyUpgrading => GameUiText.AnotherUpgradeInProgress;
    public static string RequirementsTitle => GameUiText.RequirementsTitle;
    public static string NoRequirements => GameUiText.NoRequirements;
    public static string CurrentBonus => GameUiText.CurrentBonus;
    public static string NextBonus => GameUiText.NextBonus;
}
