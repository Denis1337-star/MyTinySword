using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;
using Zenject;

/// <summary>
/// ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜ ˜ PluginYG2 / Yandex Games
/// ˜ ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜˜.
/// </summary>
public sealed class YandexGameEventsBridge : MonoBehaviour
{
    [Header("Scene Names")]
    private string _mainMenuSceneName = "MainMenu";
    [SerializeField]
    private List<string> _gameplaySceneNames = new()
    {
        "Level_1"
    };

    [Header("Debug")]
    [SerializeField] private bool _logEvents = true;

    private static YandexGameEventsBridge _instance;
    private static bool _gameReadySent;

    private GamePauseService _pauseService;

    private bool _gameplayStarted;
    private bool _pausedByApplication;
    private Coroutine _sceneLoadedRoutine;

    [Inject]
    private void Construct(GamePauseService pauseService)
    {
        _pauseService = pauseService;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        _sceneLoadedRoutine = StartCoroutine(HandleSceneLoadedNextFrame(SceneManager.GetActiveScene()));
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (_sceneLoadedRoutine != null)
        {
            StopCoroutine(_sceneLoadedRoutine);
            _sceneLoadedRoutine = null;
        }

        if (_instance == this)
            _instance = null;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ResumeAfterApplicationPause();
            return;
        }

        PauseByApplication();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseByApplication();
            return;
        }

        ResumeAfterApplicationPause();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (_sceneLoadedRoutine != null)
            StopCoroutine(_sceneLoadedRoutine);

        _sceneLoadedRoutine = StartCoroutine(HandleSceneLoadedNextFrame(scene));
    }

    private IEnumerator HandleSceneLoadedNextFrame(Scene scene)
    {
        // ˜˜˜ 1 ˜˜˜˜, ˜˜˜˜˜ UI ˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜.
        yield return null;

        SendGameReadyIfNeeded();
        UpdateGameplayMarkupForScene(scene.name);

        _sceneLoadedRoutine = null;
    }

    private void SendGameReadyIfNeeded()
    {
        if (_gameReadySent)
            return;

        YG2.GameReadyAPI();
        _gameReadySent = true;

        LogEvent("GameReadyAPI");
    }

    private void UpdateGameplayMarkupForScene(string sceneName)
    {
        if (IsGameplayScene(sceneName))
        {
            StartGameplay();
            return;
        }

        StopGameplay();
    }

    private bool IsGameplayScene(string sceneName)
    {
        for (int i = 0; i < _gameplaySceneNames.Count; i++)
        {
            if (_gameplaySceneNames[i] == sceneName)
                return true;
        }

        return false;
    }

    private void StartGameplay()
    {
        if (_gameplayStarted)
            return;

        YG2.GameplayStart();
        _gameplayStarted = true;
        _pausedByApplication = false;

        LogEvent("GameplayStart");
    }

    private void StopGameplay()
    {
        if (!_gameplayStarted)
            return;

        YG2.GameplayStop();
        _gameplayStarted = false;

        LogEvent("GameplayStop");
    }

    private void PauseByApplication()
    {
        if (_pausedByApplication)
            return;

        _pausedByApplication = true;

        StopGameplay();
        _pauseService.Pause(GamePauseReason.ApplicationFocus);

        LogEvent("Application Pause");
    }

    private void ResumeAfterApplicationPause()
    {
        if (!_pausedByApplication)
            return;

        _pausedByApplication = false;

        _pauseService.Resume(GamePauseReason.ApplicationFocus);

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (IsGameplayScene(currentSceneName))
            StartGameplay();

        LogEvent("Application Resume");
    }

    private void LogEvent(string eventName)
    {
        if (!_logEvents)
            return;

        Debug.Log($"[YandexGameEventsBridge] {eventName}", this);
    }
}