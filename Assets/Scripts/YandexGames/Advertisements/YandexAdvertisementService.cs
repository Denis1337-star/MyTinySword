using System;
using Zenject;
using YG;

/// <summary>
/// Реклама через PluginYG2.
/// Resume всегда на close/error; награда только из reward-callback.
/// После закрытия рекламы timeScale синхронизируется заново —
/// иначе PauseGameYG может вернуть сохранённый 0.
/// </summary>
public sealed class YandexAdvertisementService : IAdvertisementService, IInitializable, ITickable, IDisposable
{
    private readonly GamePauseService _pauseService;

    private Action _currentRewardedCallback;
    private Action _currentClosedCallback;
    private Action<string> _currentErrorCallback;
    private bool _resyncTimeScaleNextTick;

    public bool IsRewardedAdInProgress { get; private set; }

    public YandexAdvertisementService(GamePauseService pauseService)
    {
        _pauseService = pauseService;
    }

    public void Initialize()
    {
        YG2.onOpenRewardedAdv += HandleRewardedOpened;
        YG2.onCloseRewardedAdv += HandleRewardedClosed;
        YG2.onErrorRewardedAdv += HandleRewardedError;
        YG2.onOpenInterAdv += HandleInterstitialOpened;
        YG2.onCloseInterAdv += HandleInterstitialClosed;
    }

    public void Dispose()
    {
        YG2.onOpenRewardedAdv -= HandleRewardedOpened;
        YG2.onCloseRewardedAdv -= HandleRewardedClosed;
        YG2.onErrorRewardedAdv -= HandleRewardedError;
        YG2.onOpenInterAdv -= HandleInterstitialOpened;
        YG2.onCloseInterAdv -= HandleInterstitialClosed;
    }

    public void Tick()
    {
        if (!_resyncTimeScaleNextTick)
            return;

        _resyncTimeScaleNextTick = false;

        if (_pauseService.IsPaused)
            return;

        // PauseGameYG на close восстанавливает старый timeScale после нашего Resume —
        // на следующем кадре возвращаем актуальный gameplay-масштаб (1x / 2x).
        _pauseService.SetGameplayTimeScale(_pauseService.GameplayTimeScale);
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

        // Не ставим Pause до Open: иначе YG сохранит timeScale=0 и вернёт его при закрытии.
        try
        {
            YG2.RewardedAdvShow(rewardId, OnRewardedGranted);
        }
        catch (Exception exception)
        {
            FinishWithError(exception.Message);
        }
    }

    private void OnRewardedGranted()
    {
        if (!IsRewardedAdInProgress)
            return;

        _currentRewardedCallback?.Invoke();
    }

    private void HandleRewardedOpened()
    {
        _pauseService.Pause(GamePauseReason.Advertisement);
    }

    private void HandleRewardedClosed()
    {
        if (!IsRewardedAdInProgress)
            return;

        FinishClosed();
    }

    private void HandleRewardedError()
    {
        if (!IsRewardedAdInProgress)
            return;

        FinishWithError("Rewarded ad failed.");
    }

    private void HandleInterstitialOpened()
    {
        _pauseService.Pause(GamePauseReason.Advertisement);
    }

    private void HandleInterstitialClosed()
    {
        _pauseService.Resume(GamePauseReason.Advertisement);
        _resyncTimeScaleNextTick = true;
    }

    private void FinishClosed()
    {
        _pauseService.Resume(GamePauseReason.Advertisement);
        _resyncTimeScaleNextTick = true;

        IsRewardedAdInProgress = false;

        Action closed = _currentClosedCallback;
        ClearCallbacks();
        closed?.Invoke();
    }

    private void FinishWithError(string message)
    {
        _pauseService.Resume(GamePauseReason.Advertisement);
        _resyncTimeScaleNextTick = true;

        IsRewardedAdInProgress = false;

        Action<string> error = _currentErrorCallback;
        ClearCallbacks();
        error?.Invoke(message);
    }

    private void ClearCallbacks()
    {
        _currentRewardedCallback = null;
        _currentClosedCallback = null;
        _currentErrorCallback = null;
    }
}
