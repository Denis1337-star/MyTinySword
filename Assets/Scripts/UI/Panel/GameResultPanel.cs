using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;
using Zenject;

/// <summary>
/// Экран победы: далее / рестарт / меню.
/// </summary>
public sealed class GameResultPanel : ValidatedMonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";

    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private TMP_Text _nextLevelButtonText;
    [SerializeField] private TMP_Text _restartButtonText;
    [SerializeField] private TMP_Text _mainMenuButtonText;
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
        YG2.onSwitchLang += HandleLanguageSwitched;
        RefreshStaticTexts();
    }

    private void OnDisable()
    {
        _nextLevelButton.onClick.RemoveListener(LoadNextLevel);
        _restartButton.onClick.RemoveListener(RestartLevel);
        _mainMenuButton.onClick.RemoveListener(LoadMainMenu);
        YG2.onSwitchLang -= HandleLanguageSwitched;
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

        RefreshStaticTexts();
        _nextLevelButton.gameObject.SetActive(nextLevelConfig != null);

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void HandleLanguageSwitched(string lang)
    {
        if (!gameObject.activeInHierarchy)
            return;

        RefreshStaticTexts();
    }

    private void RefreshStaticTexts()
    {
        _resultText.text = GameUiText.Victory;

        if (_nextLevelButtonText != null)
            _nextLevelButtonText.text = GameUiText.Next;

        if (_restartButtonText != null)
            _restartButtonText.text = GameUiText.Restart;

        if (_mainMenuButtonText != null)
            _mainMenuButtonText.text = GameUiText.MainMenu;
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
