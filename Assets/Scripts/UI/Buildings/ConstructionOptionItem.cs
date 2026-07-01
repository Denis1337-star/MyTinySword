using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Элемент списка зданий в ConstructionPanel.
/// </summary>
public sealed class ConstructionOptionItem : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _button;

    public Button Button => _button;

    [SerializeField] private GameObject _selectedFrame;

    private BuildingConfig _config;
    private Action<BuildingConfig> _onSelected;
    private Func<BuildingConfig, bool> _canSelect;

    public BuildingConfig Config => _config;

    public RectTransform RectTransform => transform as RectTransform;

    private void Awake()
    {
        if (_button != null)
            _button.onClick.AddListener(HandleClicked);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClicked);
    }

    public void Bind(
        BuildingConfig config,
        Action<BuildingConfig> onSelected,
        Func<BuildingConfig, bool> canSelect)
    {
        _config = config;
        _onSelected = onSelected;
        _canSelect = canSelect;

        if (_iconImage != null)
            _iconImage.sprite = config != null ? config.Icon : null;

        RefreshInteractable();
    }

    public void SetSelected(bool selected)
    {
        if (_selectedFrame != null)
            _selectedFrame.SetActive(selected);
    }

    public void RefreshInteractable()
    {
        if (_button == null)
            return;

        _button.interactable = _canSelect == null || _canSelect.Invoke(_config);
    }

    private void HandleClicked()
    {
        if (_config == null)
            return;

        if (_canSelect != null && !_canSelect(_config))
            return;

        _onSelected?.Invoke(_config);
    }
}
