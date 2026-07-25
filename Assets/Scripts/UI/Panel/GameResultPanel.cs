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
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    private LevelConfig _nextLevelConfig;
    private LevelLoaderService _levelLoaderService;
    private GamePauseService _pauseService;
    private bool _isShowingVictory;
    private bool _isShowingDefeat;

    [Inject]
    private void Construct(
        LevelLoaderService levelLoaderService,
        GamePauseService pauseService)
    {
        _levelLoaderService = levelLoaderService;
        _pauseService = pauseService;
    }

    private void OnEnable()
    {
        _nextLevelButton.onClick.AddListener(LoadNextLevel);
        _restartButton.onClick.AddListener(RestartLevel);
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
        YG2.onSwitchLang += HandleSwitchLang;
    }

    private void OnDisable()
    {
        _nextLevelButton.onClick.RemoveListener(LoadNextLevel);
        _restartButton.onClick.RemoveListener(RestartLevel);
        _mainMenuButton.onClick.RemoveListener(LoadMainMenu);
        YG2.onSwitchLang -= HandleSwitchLang;
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
        _isShowingVictory = true;

        _resultText.text = GameUiText.Victory;
        _nextLevelButton.gameObject.SetActive(nextLevelConfig != null);

        gameObject.SetActive(true);

        YandexGameEventsBridge.NotifyGameplayResultOpened();
        _pauseService.Pause(GamePauseReason.GameResult);
    }
    public void ShowDefeat()
    {
        _nextLevelConfig = null;
        _isShowingDefeat = true;
        _isShowingVictory =false;

        _resultText.text = GameUiText.Defeat;
        _nextLevelButton.gameObject.SetActive(false);

        gameObject.SetActive(true);

        YandexGameEventsBridge.NotifyGameplayResultOpened();
        _pauseService.Pause(GamePauseReason.GameResult);
    }

    private void HandleSwitchLang(string lang)
    {
        if (_isShowingVictory)
        _resultText.text = GameUiText.Victory;
        else if(_isShowingDefeat)
            _resultText.text= GameUiText.Defeat;
    }

    private void LeaveResultScreen()
    {
        _isShowingVictory = false;
        _isShowingDefeat = false;
        _pauseService.Resume(GamePauseReason.GameResult);
    }

    private void LoadNextLevel()
    {
        if (_nextLevelConfig == null)
            return;

        LeaveResultScreen();
        _levelLoaderService.TryLoadLevel(_nextLevelConfig);
    }

    private void RestartLevel()
    {
        LeaveResultScreen();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        LeaveResultScreen();
        SceneManager.LoadScene(MainMenuSceneName);
    }
}
