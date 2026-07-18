using System.Collections.Generic;
using UnityEngine;
using YG;

/// <summary>
/// Сервис постоянного прогресса уровней.
/// Отвечает за отметку пройденных уровней и открытие следующих.
/// </summary>
public sealed class LevelProgressService
{
    private const int FirstLevelIndex = 1;

    public int LastUnlockedLevelIndex
    {
        get
        {
            EnsureInitialized();
            return YG2.saves.lastUnlockedLevelIndex;
        }
    }

    public int TotalVictories
    {
        get
        {
            EnsureInitialized();
            return YG2.saves.totalVictories;
        }
    }

    public void CompleteLevel(string levelId, int levelIndex)
    {
        if (string.IsNullOrWhiteSpace(levelId))
        {
            Debug.LogError("[LevelProgressService] Level Id пустой. Прогресс не сохранён.");
            return;
        }

        if (levelIndex < FirstLevelIndex)
        {
            Debug.LogError($"[LevelProgressService] Некорректный индекс уровня: {levelIndex}.");
            return;
        }

        EnsureInitialized();

        if (!YG2.saves.completedLevelIds.Contains(levelId))
            YG2.saves.completedLevelIds.Add(levelId);

        YG2.saves.totalVictories++;

        int nextLevelIndex = levelIndex + 1;

        if (nextLevelIndex > YG2.saves.lastUnlockedLevelIndex)
            YG2.saves.lastUnlockedLevelIndex = nextLevelIndex;

        YandexSaveUtility.SaveProgress();
    }

    public bool IsLevelCompleted(string levelId)
    {
        if (string.IsNullOrWhiteSpace(levelId))
            return false;

        EnsureInitialized();

        return YG2.saves.completedLevelIds.Contains(levelId);
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        EnsureInitialized();
        return levelIndex <= YG2.saves.lastUnlockedLevelIndex;
    }

    private void EnsureInitialized()
    {
        if (YG2.saves.levelProgressInitialized)
            return;

        YG2.saves.levelProgressInitialized = true;
        YG2.saves.lastUnlockedLevelIndex = FirstLevelIndex;
        YG2.saves.totalVictories = 0;
        YG2.saves.completedLevelIds ??= new List<string>();

        YandexSaveUtility.SaveProgress();
    }
}
