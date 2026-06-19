using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Подтверждение сноса здания.
/// </summary>
public sealed class BuildingDemolishConfirmPanel : ValidatedMonoBehaviour
{
    private const string DefaultMessage =
        "Вы уверены в сносе здания?\nРесурсы не будут возвращены";

    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private SimplePanelTween _panelTween;
    [SerializeField] private string _message = DefaultMessage;

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
        valid &= ValidationUtility.IsAssigned(this, _cancelButton, nameof(_cancelButton));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));

        return valid;
    }

    public void Show(Action onConfirm, Action onCancel)
    {
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        _messageText.text = string.IsNullOrWhiteSpace(_message) ? DefaultMessage : _message;
        _panelTween.Show();
    }

    public void Hide()
    {
        _onConfirm = null;
        _onCancel = null;

        _panelTween.Hide();
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
