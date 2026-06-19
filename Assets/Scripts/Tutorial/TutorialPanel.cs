using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-панель туториала.
/// Показывает текст подсказки, шаг и кнопки управления.
/// </summary>
public sealed class TutorialPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Text")]
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private TMP_Text _stepCounterText;

    [Header("Buttons")]
    [SerializeField] private Button _nextButton;

    public Button NextButton => _nextButton;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));
        valid &= ValidationUtility.IsAssigned(this, _canvasGroup, nameof(_canvasGroup));
        valid &= ValidationUtility.IsAssigned(this, _messageText, nameof(_messageText));
        valid &= ValidationUtility.IsAssigned(this, _stepCounterText, nameof(_stepCounterText));
        valid &= ValidationUtility.IsAssigned(this, _nextButton, nameof(_nextButton));

        return valid;
    }

    public void ShowStep(string message, int currentStepIndex, int totalSteps)
    {
        _messageText.text = message;
        _stepCounterText.text = $"{currentStepIndex + 1}/{totalSteps}";

        _panelTween.Show();
    }

    public void ShowInfo(string message, int currentStepIndex, int totalSteps)
    {
        _messageText.text = message;
        _stepCounterText.text = $"{currentStepIndex + 1}/{totalSteps}";
        _nextButton.gameObject.SetActive(false);

        gameObject.SetActive(true);

        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void Hide()
    {
        _panelTween.Hide();
    }

    public void HideImmediate()
    {
        _panelTween.HideImmediate();
    }
}
