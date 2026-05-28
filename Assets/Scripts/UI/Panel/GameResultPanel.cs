using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Показывает экран победы или поражения
/// </summary>
public sealed class GameResultPanel : ValidatedMonoBehaviour
{
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

    }

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(RestartLevel);
        _mainMenuButton.onClick.AddListener(MainMenu);
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(RestartLevel);
        _mainMenuButton.onClick.RemoveListener(MainMenu);
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
        _resultText.text = "ПОБЕДА";
        gameObject.SetActive(true);
        Time.timeScale = 0;

    }

    public void ShowDefeat()
    {
        _resultText.text = "ПОРАЖЕНИЕ";
        gameObject.SetActive(true);
        Time.timeScale = 0;

    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}