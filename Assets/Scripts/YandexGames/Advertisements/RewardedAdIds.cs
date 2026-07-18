/// <summary>
/// ID наград для YG2.RewardedAdvShow.
/// Это идентификаторы для колбэка в игре (какую награду выдать).
/// В Консоли Яндекс Игр отдельная регистрация таких id обычно не нужна —
/// достаточно подключённой Rewarded рекламы РСЯ.
/// Меняй только вместе с _rewardAmount на кнопке.
/// </summary>
public static class RewardedAdIds
{
    public const int DefaultRewardAmount = 40;

    /// <summary>
    /// Награда: заряд x2 скорости на 3 минуты (см. GameSpeedBoostService).
    /// </summary>
    public const string SpeedBoost2x = "speed_boost_2x";

    public static string ForResource(ResourceType resourceType, int amount)
    {
        return resourceType switch
        {
            ResourceType.Wood => $"wood_{amount}",
            ResourceType.Meat => $"meat_{amount}",
            ResourceType.Gold => $"gold_{amount}",
            _ => $"resource_{amount}"
        };
    }
}
