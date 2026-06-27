using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Сервис загрузки уровней.
/// Проверяет прогресс игрока, запоминает выбранный LevelConfig
/// и грузит сцену уровня.
/// </summary>
public sealed class LevelLoaderService
{
    private readonly LevelProgressService _levelProgressService;
    private readonly LevelRuntimeService _levelRuntimeService;

    public LevelLoaderService(
        LevelProgressService levelProgressService,
        LevelRuntimeService levelRuntimeService)
    {
        _levelProgressService = levelProgressService;
        _levelRuntimeService = levelRuntimeService;
    }

    public bool CanLoadLevel(LevelConfig levelConfig)
    {
        if (levelConfig == null)
            return false;

        return _levelProgressService.IsLevelUnlocked(levelConfig.LevelIndex);
    }

    public bool TryLoadLevel(LevelConfig levelConfig)
    {
        if (levelConfig == null)
        {
            Debug.LogError("[LevelLoaderService] LevelConfig не задан.");
            return false;
        }

        if (!CanLoadLevel(levelConfig))
        {
            Debug.LogWarning(
                $"[LevelLoaderService] Уровень закрыт: {levelConfig.DisplayName}. " +
                $"Нужен индекс {levelConfig.LevelIndex}, открыт до {_levelProgressService.LastUnlockedLevelIndex}.");

            return false;
        }

        // Запоминаем выбранный уровень до загрузки сцены.
        // После загрузки Level_1 другие системы смогут узнать, какой LevelConfig активен.
        _levelRuntimeService.SetCurrentLevel(levelConfig);

        Time.timeScale = 1f;
        SceneManager.LoadScene(levelConfig.SceneName);

        return true;
    }
}