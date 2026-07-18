using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ����� ���������� ������ gameplay.
/// �������� ����� ����� ������ �����, ����� ������ ������� �� �������������.
/// ������� Time.timeScale: 0 �� �����, ����� gameplay-������� (1 ��� 2 �� �����).
/// </summary>
public sealed class GamePauseService
{
    private readonly HashSet<GamePauseReason> _activeReasons = new();

    private float _previousAudioVolume = 1f;
    private bool _pauseApplied;
    private float _gameplayTimeScale = 1f;

    public event Action<bool> PauseStateChanged;

    public bool IsPaused => _activeReasons.Count > 0;
    public float GameplayTimeScale => _gameplayTimeScale;

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

    /// <summary>
    /// ������� ������� ��� ����� (������� �������� ��� x2).
    /// </summary>
    public void SetGameplayTimeScale(float timeScale)
    {
        _gameplayTimeScale = Mathf.Max(0f, timeScale);

        if (!_pauseApplied)
            Time.timeScale = _gameplayTimeScale;
    }

    public void RefreshTimeScale()
    {
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
        if (_pauseApplied)
        {
            _pauseApplied = false;
            AudioListener.pause = false;
            AudioListener.volume = _previousAudioVolume;
        }

        Time.timeScale = _gameplayTimeScale;
    }
}
