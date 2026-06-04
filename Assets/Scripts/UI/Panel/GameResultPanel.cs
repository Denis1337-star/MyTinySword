using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Показывает экран победы или поражения
/// </summary>
public sealed class GameResultPanel : ValidatedMonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";

    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(RestartLevel);
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(RestartLevel);
        _mainMenuButton.onClick.RemoveListener(LoadMainMenu);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _resultText, nameof(_resultText));
        valid &= ValidationUtility.IsAssigned(this, _restartButton, nameof(_restartButton));
        valid &= ValidationUtility.IsAssigned(this, _mainMenuButton, nameof(_mainMenuButton));

        return valid;
    }

    public void ShowVictory()
    {
        ShowResult("ПОБЕДА");
    }

    public void ShowDefeat()
    {
        ShowResult("ПОРАЖЕНИЕ");
    }

    private void ShowResult(string resultText)
    {
        _resultText.text = resultText;
        gameObject.SetActive(true);

        Time.timeScale = 0f;
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuSceneName);
    }
}