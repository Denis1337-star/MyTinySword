using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Универсальная панель подтверждения сноса здания.
/// Умеет показывать как разрешённый снос, так и причину запрета.
/// </summary>
public sealed class BuildingDemolishConfirmPanel : ValidatedMonoBehaviour
{
    private const string DefaultAllowedMessage =
        "Вы уверены, что хотите снести здание?\nРесурсы не будут возвращены.";

    private const string DefaultBlockedMessage =
        "Это здание нельзя снести.";

    [Header("Text")]
    [SerializeField] private TMP_Text _messageText;

    [Header("Buttons")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TMP_Text _confirmButtonText;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private TMP_Text _cancelButtonText;

    [Header("Animation")]
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("Messages")]
    [SerializeField, TextArea] private string _allowedMessage = DefaultAllowedMessage;
    [SerializeField, TextArea] private string _blockedMessage = DefaultBlockedMessage;

    private Action _onConfirm;
    private Action _onCancel;

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(HandleConfirmClicked);
        _cancelButton.onClick.AddListener(HandleCancelClicked);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(HandleConfirmClicked);
        _cancelButton.onClick.RemoveListener(HandleCancelClicked);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _messageText, nameof(_messageText));
        valid &= ValidationUtility.IsAssigned(this, _confirmButton, nameof(_confirmButton));
        valid &= ValidationUtility.IsAssigned(this, _confirmButtonText, nameof(_confirmButtonText));
        valid &= ValidationUtility.IsAssigned(this, _cancelButton, nameof(_cancelButton));
        valid &= ValidationUtility.IsAssigned(this, _cancelButtonText, nameof(_cancelButtonText));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));

        return valid;
    }

    public void ShowAllowed(
        string message,
        Action onConfirm,
        Action onCancel)
    {
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        _messageText.text = string.IsNullOrWhiteSpace(message)
            ? GetAllowedMessage()
            : message;

        _confirmButton.gameObject.SetActive(true);
        _confirmButton.interactable = true;
        _confirmButtonText.text = "Да, снести";

        _cancelButton.gameObject.SetActive(true);
        _cancelButton.interactable = true;
        _cancelButtonText.text = "Выйти";

        _panelTween.Show();
    }

    public void ShowBlocked(string message, Action onCancel)
    {
        _onConfirm = null;
        _onCancel = onCancel;

        _messageText.text = string.IsNullOrWhiteSpace(message)
            ? GetBlockedMessage()
            : message;

        _confirmButton.gameObject.SetActive(false);

        _cancelButton.gameObject.SetActive(true);
        _cancelButton.interactable = true;
        _cancelButtonText.text = "Выйти";

        _panelTween.Show();
    }

    public void Hide()
    {
        _onConfirm = null;
        _onCancel = null;

        _panelTween.Hide();
    }

    private string GetAllowedMessage()
    {
        return string.IsNullOrWhiteSpace(_allowedMessage)
            ? DefaultAllowedMessage
            : _allowedMessage;
    }

    private string GetBlockedMessage()
    {
        return string.IsNullOrWhiteSpace(_blockedMessage)
            ? DefaultBlockedMessage
            : _blockedMessage;
    }

    private void HandleConfirmClicked()
    {
        _onConfirm?.Invoke();
    }

    private void HandleCancelClicked()
    {
        _onCancel?.Invoke();
    }
}