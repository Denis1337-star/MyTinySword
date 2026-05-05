using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-элемент выбора здания в панели строительства
/// </summary>
public class ConstructionOptionItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedFrame;

    private BuildingConfig _config;
    private Action<BuildingConfig> _onSelected;

    public BuildingConfig Config => _config;

    private void OnEnable()
    {
        _button?.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        _button?.onClick.RemoveListener(HandleClick);
    }

    /// <summary>
    /// Привязывает item к config здания
    /// </summary>
    public void Bind(BuildingConfig config, Action<BuildingConfig> onSelected)
    {
        _config = config;
        _onSelected = onSelected;

        if (_iconImage != null)
            _iconImage.sprite = _config != null ? _config.Icon : null;

        SetSelected(false);
    }

    /// <summary>
    /// Включает или выключает рамку выбранного здания
    /// </summary>
    public void SetSelected(bool value)
    {
        if (_selectedFrame != null)
            _selectedFrame.SetActive(value);
    }

    private void HandleClick()
    {
        if (_config == null)
            return;

        _onSelected?.Invoke(_config);
    }

}