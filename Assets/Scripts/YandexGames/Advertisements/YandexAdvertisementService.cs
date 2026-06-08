using System;
using UnityEngine;
using YG;

/// <summary>
/// –еализаци€ рекламы через PluginYG2 
/// —тавит игру на паузу на врем€ рекламы и выдаЄт награду только по rewarded callback
/// </summary>
public sealed class YandexAdvertisementService : IAdvertisementService
{
    private readonly GamePauseService _pauseService;

    private Action _currentRewardedCallback;
    private Action _currentClosedCallback;
    private Action<string> _currentErrorCallback;

    public bool IsRewardedAdInProgress { get; private set; }

    public YandexAdvertisementService(GamePauseService pauseService)
    {
        _pauseService = pauseService;
    }

    public void ShowRewardedAd(
        string rewardId,
        Action onRewarded,
        Action onClosed = null,
        Action<string> onError = null)
    {
        if (string.IsNullOrWhiteSpace(rewardId))
        {
            onError?.Invoke("Reward id is empty.");
            return;
        }

        if (IsRewardedAdInProgress)
        {
            onError?.Invoke("Rewarded ad is already in progress.");
            return;
        }

        IsRewardedAdInProgress = true;

        _currentRewardedCallback = onRewarded;
        _currentClosedCallback = onClosed;
        _currentErrorCallback = onError;

        _pauseService.Pause(GamePauseReason.Advertisement);

        try
        {
            // ѕервый параметр Ч id награды.
            // ¬торой параметр Ч callback награды.
            YG2.RewardedAdvShow(rewardId, OnRewarded);
        }
        catch (Exception exception)
        {
            FinishWithError(exception.Message);
        }
    }

    private void OnRewarded()
    {
        _currentRewardedCallback?.Invoke();
        FinishSuccessfully();
    }

    private void FinishSuccessfully()
    {
        _pauseService.Resume(GamePauseReason.Advertisement);

        IsRewardedAdInProgress = false;

        _currentClosedCallback?.Invoke();

        ClearCallbacks();
    }

    private void FinishWithError(string message)
    {
        _pauseService.Resume(GamePauseReason.Advertisement);

        IsRewardedAdInProgress = false;

        _currentErrorCallback?.Invoke(message);

        ClearCallbacks();
    }

    private void ClearCallbacks()
    {
        _currentRewardedCallback = null;
        _currentClosedCallback = null;
        _currentErrorCallback = null;
    }
}