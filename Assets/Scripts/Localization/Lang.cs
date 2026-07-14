using YG;

/// <summary>
/// Хелпер текущего языка PluginYG2.
/// </summary>
public static class Lang
{
    public const string Russian = "ru";
    public const string English = "en";

    public static string Current => YG2.lang;

    public static bool IsEnglish => Current == English;

    public static string Pick(string ru, string en)
    {
        if (IsEnglish && !string.IsNullOrWhiteSpace(en))
            return en;

        return ru;
    }

    public static string PickDisplayName(string ru, string en) => Pick(ru, en);
}
