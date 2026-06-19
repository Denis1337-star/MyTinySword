using System.Collections.Generic;
using YG;

/// <summary>
/// Значения по умолчанию для YG2.saves.
/// </summary>
public static class GameSavesDefaults
{
    public const float MusicVolume = 1f;
    public const float SfxVolume = 1f;
    public const int FirstLevelIndex = 1;

    public static void ApplyAudio(SavesYG saves)
    {
        saves.audioSettingsInitialized = false;
        saves.musicVolume = MusicVolume;
        saves.sfxVolume = SfxVolume;
        saves.musicMuted = false;
        saves.sfxMuted = false;
    }

    public static void ApplyLevelProgress(SavesYG saves)
    {
        saves.levelProgressInitialized = false;
        saves.lastUnlockedLevelIndex = FirstLevelIndex;
        saves.totalVictories = 0;
        saves.completedLevelIds = new List<string>();
    }

    public static void ApplyTutorial(SavesYG saves)
    {
        saves.tutorialCompleted = false;
    }

    public static void ApplyAll(SavesYG saves)
    {
        ApplyAudio(saves);
        ApplyLevelProgress(saves);
        ApplyTutorial(saves);
    }
}
