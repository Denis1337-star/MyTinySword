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
        GameSavesDefaults.ApplyAll(YG2.saves);
        YandexSaveUtility.SaveProgress();

        Debug.Log("[DebugResetAllSavesButton] Все сохранения проекта сброшены.");

        if (_reloadSceneAfterReset)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
