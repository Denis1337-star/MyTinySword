using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

/// <summary>
/// Временная debug-кнопка для сброса всех наших сохранений.
/// Перед релизом удалить из сцены или отключить.
/// </summary>
public sealed class DebugResetAllSavesButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private bool _reloadSceneAfterReset = true;

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button != null)
            _button.onClick.AddListener(ResetAllSaves);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(ResetAllSaves);
    }

    private void ResetAllSaves()
    {
        ResetAudioSaves();
        ResetLevelProgressSaves();
        ResetTutorialSaves();

        YG2.SaveProgress();

        Debug.Log("[DebugResetAllSavesButton] Все сохранения проекта сброшены.");

        if (_reloadSceneAfterReset)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ResetAudioSaves()
    {
        YG2.saves.audioSettingsInitialized = false;
        YG2.saves.musicVolume = 1f;
        YG2.saves.sfxVolume = 1f;
        YG2.saves.musicMuted = false;
        YG2.saves.sfxMuted = false;
    }

    private void ResetLevelProgressSaves()
    {
        YG2.saves.levelProgressInitialized = false;
        YG2.saves.lastUnlockedLevelIndex = 1;
        YG2.saves.totalVictories = 0;
        YG2.saves.completedLevelIds = new List<string>();
    }

    private void ResetTutorialSaves()
    {
        YG2.saves.tutorialCompleted = false;
    }
}