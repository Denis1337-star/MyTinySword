using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Завершает уровень при уничтожении вражеского замка.
/// Сохраняет прогресс и показывает экран победы.
/// </summary>
public sealed class GameResultController : ValidatedMonoBehaviour
{
    [Header("Level")]
    [SerializeField] private LevelConfig _fallbackLevelConfig;
    [SerializeField] private LevelCatalog _levelCatalog;

    [Header("Castles")]
    [SerializeField] private Castle _enemyCastle;
    [SerializeField] private Castle _playerCastle;

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

        LevelConfig level = GetCurrentLevelConfig();

        if (_playerCastle != null)
            _playerCastle.OnCastleDestroyed += OnPlayerCastleDestroyed;

        if (level != null
            && level.ObjectiveType == LevelObjectiveType.DestroyEnemyCastle
            && _enemyCastle != null)
        {
            _enemyCastle.OnCastleDestroyed += OnEnemyCastleDestroyed;
        }
    }

    private void OnDestroy()
    {
        if (_playerCastle != null)
            _playerCastle.OnCastleDestroyed -= OnPlayerCastleDestroyed;

        if (_enemyCastle != null)
            _enemyCastle.OnCastleDestroyed -= OnEnemyCastleDestroyed;

        GameFinished = null;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _levelCatalog, nameof(_levelCatalog));
        valid &= ValidationUtility.IsValidConfig(this, _levelCatalog, nameof(_levelCatalog));
        valid &= ValidationUtility.IsAssigned(this, _resultPanel, nameof(_resultPanel));

        if (_fallbackLevelConfig != null)
            valid &= _fallbackLevelConfig.IsValid();

        return valid;
    }

    private void OnEnemyCastleDestroyed(Castle _)
    {
        if (_gameFinished)
            return;

        FinishVictory();
    }
    private void OnPlayerCastleDestroyed(Castle _)
    {
        if(_gameFinished) return;

        FinishDefeat();
    }

    public void FinishVictory()
    {
        _gameFinished = true;

        LevelConfig currentLevel = GetCurrentLevelConfig();
        SaveVictoryProgress(currentLevel);

        LevelConfig nextLevel = GetNextLevelConfig(currentLevel);
        _resultPanel.ShowVictory(nextLevel);

        GameFinished?.Invoke(true);
    }
    public void FinishDefeat()
    {
        _gameFinished = true;
        _resultPanel.ShowDefeat();
        GameFinished?.Invoke(false);
    }

    private void SaveVictoryProgress(LevelConfig levelConfig)
    {
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

    private LevelConfig GetNextLevelConfig(LevelConfig currentLevel)
    {
        if (currentLevel == null)
            return null;

        return _levelCatalog.GetByIndex(currentLevel.LevelIndex + 1);
    }
}