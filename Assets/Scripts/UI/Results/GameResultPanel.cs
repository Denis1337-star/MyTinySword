using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Показывает экран победы или поражения
/// </summary>
public class GameResultPanel : MonoBehaviour
{
    [SerializeField] private Text resultText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartLevel);
    }

    public void ShowVictory()
    {
        if (resultText != null)
            resultText.text = "ПОБЕДА";

        gameObject.SetActive(true);
    }

    public void ShowDefeat()
    {
        if (resultText != null)
            resultText.text = "ПОРАЖЕНИЕ";

        gameObject.SetActive(true);
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}