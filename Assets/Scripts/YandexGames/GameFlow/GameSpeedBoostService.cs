using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

/// <summary>
/// Заряд x2 скорости после rewarded-рекламы.
/// Оставшееся время живёт между уровнями (ProjectContext).
/// Списывается только пока буст включён на уровне.
/// </summary>
public sealed class GameSpeedBoostService : IInitializable, ITickable, IDisposable
{
    public const float DefaultRewardSeconds = 180f;
    public const float ActiveTimeScale = 2f;
    public const float NormalTimeScale = 1f;

    private readonly GamePauseService _pauseService;

    private float _remainingSeconds;
    private bool _isEnabled;

    public event Action StateChanged;

    public float RemainingSeconds => Mathf.Max(0f, _remainingSeconds);
    public bool HasCharge => _remainingSeconds > 0.01f;
    public bool IsEnabled => _isEnabled && HasCharge;
    public bool IsGameplaySceneActive => IsGameplayScene(SceneManager.GetActiveScene().name);

    public GameSpeedBoostService(GamePauseService pauseService)
    {
        _pauseService = pauseService;
    }

    public void Initialize()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        _pauseService.PauseStateChanged += HandlePauseStateChanged;
        ApplyTimeScale();
    }

    public void Dispose()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        _pauseService.PauseStateChanged -= HandlePauseStateChanged;
    }

    public void Tick()
    {
        if (!_pauseService.IsPaused && IsGameplaySceneActive)
        {
            float expected = IsEnabled ? ActiveTimeScale : NormalTimeScale;
            if (!Mathf.Approximately(Time.timeScale, expected))
                ApplyTimeScale();
        }

        if (!IsEnabled || _pauseService.IsPaused || !IsGameplaySceneActive)
            return;

        _remainingSeconds -= Time.unscaledDeltaTime;

        if (_remainingSeconds > 0f)
            return;

        _remainingSeconds = 0f;
        _isEnabled = false;
        ApplyTimeScale();
        StateChanged?.Invoke();
    }

    public void GrantReward(float durationSeconds = DefaultRewardSeconds)
    {
        if (durationSeconds <= 0f)
            durationSeconds = DefaultRewardSeconds;

        _remainingSeconds += durationSeconds;
        _isEnabled = true;
        ApplyTimeScale();
        StateChanged?.Invoke();
    }

    public void SetEnabled(bool enabled)
    {
        if (!HasCharge)
        {
            _isEnabled = false;
            ApplyTimeScale();
            StateChanged?.Invoke();
            return;
        }

        if (_isEnabled == enabled)
            return;

        _isEnabled = enabled;
        ApplyTimeScale();
        StateChanged?.Invoke();
    }

    public void ToggleEnabled()
    {
        SetEnabled(!IsEnabled);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsGameplayScene(scene.name))
            _isEnabled = false;

        ApplyTimeScale();
        StateChanged?.Invoke();
    }

    private void HandlePauseStateChanged(bool isPaused)
    {
        ApplyTimeScale();
    }

    private void ApplyTimeScale()
    {
        bool boostActive = IsEnabled && IsGameplaySceneActive;
        float gameplayScale = boostActive ? ActiveTimeScale : NormalTimeScale;
        _pauseService.SetGameplayTimeScale(gameplayScale);
    }

    public static bool IsGameplayScene(string sceneName)
    {
        return !string.IsNullOrEmpty(sceneName) && sceneName.StartsWith("Level_", StringComparison.Ordinal);
    }
}
