using UnityEngine;
using YG;

/// <summary>
/// Сервис сохранения состояния обучения.
/// </summary>
public sealed class TutorialSaveService
{
    public bool IsTutorialCompleted()
    {
        return YG2.saves.tutorialCompleted;
    }

    public void MarkTutorialCompleted()
    {
        if (YG2.saves.tutorialCompleted)
            return;

        YG2.saves.tutorialCompleted = true;
        YandexSaveUtility.SaveProgress();
    }

    public void ResetTutorial()
    {
        GameSavesDefaults.ApplyTutorial(YG2.saves);
        YandexSaveUtility.SaveProgress();
    }
}
