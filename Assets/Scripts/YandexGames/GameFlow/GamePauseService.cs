using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Единая точка управления паузой gameplay
/// </summary>
public sealed class GamePauseService
{
    private readonly HashSet<GamePauseReason> _activeReasons = new();

    private float _previousAudioVolume = 1f;

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
            ApplyPause();
        else
            ApplyResume();

        PauseStateChanged?.Invoke(shouldPause);
    }

    private void ApplyPause()
    {
        Time.timeScale = 0f;

        _previousAudioVolume = AudioListener.volume;
        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }

    private void ApplyResume()
    {
        Time.timeScale = 1f;

        AudioListener.pause = false;
        AudioListener.volume = _previousAudioVolume;
    }
}