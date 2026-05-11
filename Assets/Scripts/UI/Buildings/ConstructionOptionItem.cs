using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI элемент выбора здания в панели строительства
/// </summary>
public sealed class ConstructionOptionItem : ValidatedMonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedFrame;

    private BuildingConfig _config;
    private Action<BuildingConfig> _onSelected;

    public BuildingConfig Config => _config;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _iconImage, nameof(_iconImage));
        valid &= ValidationUtility.IsAssigned(this, _button, nameof(_button));
        valid &= ValidationUtility.IsAssigned(this, _selectedFrame, nameof(_selectedFrame));

        return valid;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(HandleClick);
    }

    /// <summary>
    /// Привязывает item к config здания
    /// </summary>
    public void Bind(BuildingConfig config, Action<BuildingConfig> onSelected)
    {
        _config = config;
        _onSelected = onSelected;

        _iconImage.sprite = _config != null ? _config.Icon : null;
        _button.interactable = _config != null;

        SetSelected(false);
    }

    public void SetSelected(bool value)
    {
        _selectedFrame.SetActive(value);
    }

    private void HandleClick()
    {
        if (_config == null)
            return;

        _onSelected?.Invoke(_config);
    }
}