using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ”правл€ет главным меню игры
/// </summary>
public sealed class MainMenuController : ValidatedMonoBehaviour
{
    [SerializeField] private string _gameSceneName;
    [SerializeField] private Button _startButton;

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        _startButton.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(StartGame);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _startButton, nameof(_startButton));

        if (string.IsNullOrWhiteSpace(_gameSceneName))
        {
            Debug.LogError($"{name}: им€ игровой сцены не задано.", this);
            valid = false;
        }

        return valid;
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_gameSceneName);
    }
}