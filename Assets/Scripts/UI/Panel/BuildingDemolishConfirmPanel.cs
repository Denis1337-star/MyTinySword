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

    private Func<string> _messageProvider;
    private Action _onConfirm;
    private Action _onCancel;
    private bool _isAllowedMode;

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(HandleConfirmClicked);
        _cancelButton.onClick.AddListener(HandleCancelClicked);
        YG2.onSwitchLang += HandleSwitchLang;
        RefreshTexts();
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(HandleConfirmClicked);
        _cancelButton.onClick.RemoveListener(HandleCancelClicked);
        YG2.onSwitchLang -= HandleSwitchLang;
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
        Func<string> messageProvider,
        Action onConfirm,
        Action onCancel)
    {
        _isAllowedMode = true;
        _messageProvider = messageProvider ?? (() => GameUiText.DemolishConfirmNoRefund);
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        _confirmButton.gameObject.SetActive(true);
        _confirmButton.interactable = true;
        _cancelButton.gameObject.SetActive(true);
        _cancelButton.interactable = true;

        RefreshTexts();
        _panelTween.Show();
    }

    /// <summary>Совместимость: старый вызов со строкой.</summary>
    public void ShowAllowed(string message, Action onConfirm, Action onCancel)
    {
        string frozen = message;
        ShowAllowed(
            () => string.IsNullOrWhiteSpace(frozen) ? GameUiText.DemolishConfirmNoRefund : frozen,
            onConfirm,
            onCancel);
    }

    public void ShowBlocked(Func<string> messageProvider, Action onCancel)
    {
        _isAllowedMode = false;
        _messageProvider = messageProvider ?? (() => GameUiText.CannotDemolishBuilding);
        _onConfirm = null;
        _onCancel = onCancel;

        _confirmButton.gameObject.SetActive(false);
        _cancelButton.gameObject.SetActive(true);
        _cancelButton.interactable = true;

        RefreshTexts();
        _panelTween.Show();
    }

    public void ShowBlocked(string message, Action onCancel)
    {
        string frozen = message;
        ShowBlocked(
            () => string.IsNullOrWhiteSpace(frozen) ? GameUiText.CannotDemolishBuilding : frozen,
            onCancel);
    }

    public void Hide()
    {
        _messageProvider = null;
        _onConfirm = null;
        _onCancel = null;
        _panelTween.Hide();
    }

    private void HandleSwitchLang(string lang)
    {
        RefreshTexts();
    }

    private void RefreshTexts()
    {
        if (_messageText != null && _messageProvider != null)
            _messageText.text = _messageProvider();

        if (_confirmButtonText != null && _isAllowedMode)
            _confirmButtonText.text = GameUiText.YesDemolish;

        if (_cancelButtonText != null)
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
