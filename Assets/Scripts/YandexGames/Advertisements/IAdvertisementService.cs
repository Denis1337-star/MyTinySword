using System;

/// <summary>
/// Общий интерфейс показа рекламы
/// </summary>
public interface IAdvertisementService
{
    bool IsRewardedAdInProgress { get; }

    void ShowRewardedAd(
        string rewardId,
        Action onRewarded,
        Action onClosed = null,
        Action<string> onError = null);
}