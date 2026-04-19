using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Один элемент нижнего списка зданий.
/// Показывает иконку и сообщает панели, какое здание выбрано.
/// </summary>
public class ConstructionOptionItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedFrame;

    private BuildingConfig config;
    private Action<BuildingConfig> onSelected;

    public BuildingConfig Config => config;

    /// <summary>
    /// Привязывает UI-элемент к конфигу здания.
    /// </summary>
    public void Bind(BuildingConfig config, Action<BuildingConfig> onSelected)
    {
        this.config = config;
        this.onSelected = onSelected;

        if (iconImage != null)
            iconImage.sprite = config != null ? config.Icon : null;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);
        }

        SetSelected(false);
    }

    /// <summary>
    /// Включает или выключает рамку выбранного состояния.
    /// </summary>
    public void SetSelected(bool value)
    {
        if (selectedFrame != null)
            selectedFrame.SetActive(value);
    }

    private void HandleClick()
    {
        if (config == null)
            return;

        onSelected?.Invoke(config);
    }
}