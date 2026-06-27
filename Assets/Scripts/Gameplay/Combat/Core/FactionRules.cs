/// <summary>
/// Общие правила фракций
/// </summary>
public static class FactionRules
{
    public static bool IsEnemy(FactionType self, FactionType other)
    {
        return self != other;
    }

    public static bool IsPlayer(FactionType faction)
    {
        return faction == FactionType.Player;
    }

    public static bool IsEnemy(FactionType faction)
    {
        return faction == FactionType.Enemy;
    }
}