using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Единая точка управления паузой gameplay.
/// Работает через набор причин паузы, чтобы разные системы не конфликтовали.
/// </summary>
public sealed class GamePauseService
{
    private readonly HashSet<GamePauseReason> _activeReasons = new();

    private float _previousAudioVolume = 1f;
    private bool _pauseApplied;

    public event Action<bool> PauseStateChanged;

    public bool IsPaused => _activeReasons.Count > 0;

    public void Pause(GamePauseReason reason)
    {
        if (!_activeReasons.Add(reason))
            return;

        RefreshPauseState();
    }

    public void Resume(GamePauseReason reason)
    {
        if (!_activeReasons.Remove(reason))
            return;

        RefreshPauseState();
    }

    public void ResumeAll()
    {
        if (_activeReasons.Count == 0)
            return;

        _activeReasons.Clear();
        RefreshPauseState();
    }

    private void RefreshPauseState()
    {
        bool shouldPause = IsPaused;

        if (shouldPause)
            ApplyPauseIfNeeded();
        else
            ApplyResumeIfNeeded();

        PauseStateChanged?.Invoke(shouldPause);
    }

    private void ApplyPauseIfNeeded()
    {
        if (_pauseApplied)
            return;

        _pauseApplied = true;

        Time.timeScale = 0f;

        _previousAudioVolume = AudioListener.volume;

        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }

    private void ApplyResumeIfNeeded()
    {
        if (!_pauseApplied)
            return;

        _pauseApplied = false;

        Time.timeScale = 1f;

        AudioListener.pause = false;
        AudioListener.volume = _previousAudioVolume;
    }
}