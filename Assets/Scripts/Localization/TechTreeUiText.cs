using YG;

public static class TechTreeUiText
{
    public static string Upgrade =>
        YG2.lang == "ru" ? "Улучшить" : "Upgrade";

    public static string Upgrading =>
        YG2.lang == "ru" ? "Улучшается" : "Upgrading";

    public static string AlreadyUpgrading =>
        YG2.lang == "ru" ? "Уже идёт улучшение" : "Another upgrade in progress";

    public static string RequirementsTitle =>
        YG2.lang == "ru" ? "Надо прокачать" : "Requirements";

    public static string NoRequirements =>
        YG2.lang == "ru" ? "Требований нет" : "No requirements";

    public static string CurrentBonus =>
        YG2.lang == "ru" ? "Текущий бонус" : "Current bonus";

    public static string NextBonus =>
        YG2.lang == "ru" ? "Следующий бонус" : "Next bonus";
}