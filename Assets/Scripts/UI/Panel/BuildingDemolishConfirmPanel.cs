using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

/// <summary>
/// Универсальная панель подтверждения сноса здания.
/// Умеет показывать как разрешённый снос, так и причину запрета.
/// </summary>
public sealed class BuildingDemolishConfirmPanel : ValidatedMonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text _messageText;

    [Header("Buttons")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TMP_Text _confirmButtonText;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private TMP_Text _cancelButtonText;

    [Header("Animation")]
    [SerializeField] private SimplePanelTween _panelTween;

    private Action _onConfirm;
    private Action _onCancel;
    private string _customMessage;

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(HandleConfirmClicked);
        _cancelButton.onClick.AddListener(HandleCancelClicked);
        YG2.onSwitchLang += HandleLanguageSwitched;
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(HandleConfirmClicked);
        _cancelButton.onClick.RemoveListener(HandleCancelClicked);
        YG2.onSwitchLang -= HandleLanguageSwitched;
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
        _customMessage = message;

        RefreshAllowedView();
        _panelTween.Show();
    }

    public void ShowBlocked(string message, Action onCancel)
    {
        _onConfirm = null;
        _onCancel = onCancel;
        _customMessage = message;

        RefreshBlockedView();
        _panelTween.Show();
    }

    public void Hide()
    {
        _onConfirm = null;
        _onCancel = null;
        _customMessage = null;

        _panelTween.Hide();
    }

    private void HandleLanguageSwitched(string lang)
    {
        if (!_panelTween.IsVisible)
            return;

        if (_onConfirm != null)
            RefreshAllowedView();
        else
            RefreshBlockedView();
    }

    private void RefreshAllowedView()
    {
        _messageText.text = string.IsNullOrWhiteSpace(_customMessage)
            ? GameUiText.DemolishConfirmNoRefund
            : _customMessage;

        _confirmButton.gameObject.SetActive(true);
        _confirmButton.interactable = true;
        _confirmButtonText.text = GameUiText.YesDemolish;

        _cancelButton.gameObject.SetActive(true);
        _cancelButton.interactable = true;
        _cancelButtonText.text = GameUiText.Close;
    }

    private void RefreshBlockedView()
    {
        _messageText.text = string.IsNullOrWhiteSpace(_customMessage)
            ? GameUiText.CannotDemolishBuilding
            : _customMessage;

        _confirmButton.gameObject.SetActive(false);

        _cancelButton.gameObject.SetActive(true);
        _cancelButton.interactable = true;
        _cancelButtonText.text = GameUiText.Close;
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
