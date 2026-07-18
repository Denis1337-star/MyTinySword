using YG;

/// <summary>
/// Тонкая обёртка над PluginYG2: текущий язык и безопасный выбор RU/EN.
/// </summary>
public static class Lang
{
    public const string Russian = "ru";
    public const string English = "en";

    public static string Current => YG2.lang;

    public static bool IsEnglish => Current == English;

    /// <summary>
    /// Возвращает EN, если язык английский и перевод не пустой; иначе RU.
    /// </summary>
    public static string Pick(string ru, string en)
    {
        if (IsEnglish && !string.IsNullOrWhiteSpace(en))
            return en;

        return ru;
    }
}
