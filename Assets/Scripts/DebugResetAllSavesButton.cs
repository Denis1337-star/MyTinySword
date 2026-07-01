using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

/// <summary>
/// Временная debug-кнопка для сброса всех наших сохранений.
/// Перед релизом удалить из сцены или отключить.
/// </summary>
[RequireComponent(typeof(Button))]
public sealed class DebugResetAllSavesButton : ValidatedMonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private bool _reloadSceneAfterReset = true;

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _button, nameof(_button));
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ResetAllSaves);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ResetAllSaves);
    }

    private void ResetAllSaves()
    {
        GameSavesDefaults.ApplyAll(YG2.saves);
        YandexSaveUtility.SaveProgress();

        Debug.Log(
            "[DebugResetAllSavesButton] Сброшены: audio, уровни, tutorial, tech tree. " +
            "Save записан в PluginYG2.");

        if (!_reloadSceneAfterReset)
            return;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
