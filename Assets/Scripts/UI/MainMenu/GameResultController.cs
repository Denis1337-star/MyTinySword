using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Следит за уничтожением главных баз и завершает матч.
/// При победе сохраняет прогресс текущего уровня.
/// </summary>
public sealed class GameResultController : ValidatedMonoBehaviour
{
    [Header("Level")]
    [SerializeField] private LevelConfig _fallbackLevelConfig;

    [Header("Castles")]
    [SerializeField] private Castle _playerCastle;
    [SerializeField] private Castle _enemyCastle;

    [Header("UI")]
    [SerializeField] private GameResultPanel _resultPanel;

    private LevelProgressService _levelProgressService;
    private LevelRuntimeService _levelRuntimeService;

    private bool _gameFinished;

    public event Action<bool> GameFinished;

    [Inject]
    private void Construct(
        LevelProgressService levelProgressService,
        LevelRuntimeService levelRuntimeService)
    {
        _levelProgressService = levelProgressService;
        _levelRuntimeService = levelRuntimeService;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        GameFinished = null;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _playerCastle, nameof(_playerCastle));
        valid &= ValidationUtility.IsAssigned(this, _enemyCastle, nameof(_enemyCastle));
        valid &= ValidationUtility.IsAssigned(this, _resultPanel, nameof(_resultPanel));

        if (_fallbackLevelConfig != null)
            valid &= _fallbackLevelConfig.IsValid();

        return valid;
    }

    private void Subscribe()
    {
        _playerCastle.OnCastleDestroyed += OnCastleDestroyed;
        _enemyCastle.OnCastleDestroyed += OnCastleDestroyed;
    }

    private void Unsubscribe()
    {
        if (_playerCastle != null)
            _playerCastle.OnCastleDestroyed -= OnCastleDestroyed;

        if (_enemyCastle != null)
            _enemyCastle.OnCastleDestroyed -= OnCastleDestroyed;
    }

    private void OnCastleDestroyed(Castle destroyedCastle)
    {
        if (_gameFinished || destroyedCastle == null)
            return;

        if (destroyedCastle == _playerCastle)
        {
            FinishGame(false);
            return;
        }

        if (destroyedCastle == _enemyCastle)
        {
            FinishGame(true);
        }
    }

    private void FinishGame(bool victory)
    {
        _gameFinished = true;

        if (victory)
        {
            SaveVictoryProgress();
            _resultPanel.ShowVictory();

            GameFinished?.Invoke(true);
            return;
        }

        _resultPanel.ShowDefeat();

        GameFinished?.Invoke(false);
    }

    private void SaveVictoryProgress()
    {
        LevelConfig levelConfig = GetCurrentLevelConfig();

        if (levelConfig == null)
        {
            Debug.LogError($"{name}: не удалось определить текущий LevelConfig. Прогресс не сохранён.", this);
            return;
        }

        _levelProgressService.CompleteLevel(
            levelConfig.LevelId,
            levelConfig.LevelIndex);
    }

    private LevelConfig GetCurrentLevelConfig()
    {
        if (_levelRuntimeService.HasCurrentLevel)
            return _levelRuntimeService.CurrentLevel;

        return _fallbackLevelConfig;
    }
}