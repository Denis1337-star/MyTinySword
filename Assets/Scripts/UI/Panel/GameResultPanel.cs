using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Экран победы: далее / рестарт / меню.
/// </summary>
public sealed class GameResultPanel : ValidatedMonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";

    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    private LevelConfig _nextLevelConfig;
    private LevelLoaderService _levelLoaderService;

    [Inject]
    private void Construct(LevelLoaderService levelLoaderService)
    {
        _levelLoaderService = levelLoaderService;
    }

    private void OnEnable()
    {
        _nextLevelButton.onClick.AddListener(LoadNextLevel);
        _restartButton.onClick.AddListener(RestartLevel);
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void OnDisable()
    {
        _nextLevelButton.onClick.RemoveListener(LoadNextLevel);
        _restartButton.onClick.RemoveListener(RestartLevel);
        _mainMenuButton.onClick.RemoveListener(LoadMainMenu);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _resultText, nameof(_resultText));
        valid &= ValidationUtility.IsAssigned(this, _nextLevelButton, nameof(_nextLevelButton));
        valid &= ValidationUtility.IsAssigned(this, _restartButton, nameof(_restartButton));
        valid &= ValidationUtility.IsAssigned(this, _mainMenuButton, nameof(_mainMenuButton));

        return valid;
    }

    public void ShowVictory(LevelConfig nextLevelConfig)
    {
        _nextLevelConfig = nextLevelConfig;

        _resultText.text = "ПОБЕДА";
        _nextLevelButton.gameObject.SetActive(nextLevelConfig != null);

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void LoadNextLevel()
    {
        if (_nextLevelConfig == null)
            return;

        Time.timeScale = 1f;
        _levelLoaderService.TryLoadLevel(_nextLevelConfig);
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