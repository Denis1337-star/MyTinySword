using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Панель строительства.
/// Показывает список доступных зданий, информацию о выбранном здании
/// и позволяет запустить строительство.
/// </summary>
public class ConstructionPanel : MonoBehaviour
{
    [Header("Main Info")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text costText;
    [SerializeField] private Text buildTimeText;
    [SerializeField] private Image previewImage;

    [Header("Build Button")]
    [SerializeField] private Button buildButton;
    [SerializeField] private Text buildButtonText;

    [Header("Options List")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private ConstructionOptionItem optionPrefab;

    private readonly List<ConstructionOptionItem> optionItems = new();

    private ConstructionSlot currentSlot;
    private BuildingConfig selectedConfig;

    private void Awake()
    {
        if (buildButton != null)
            buildButton.onClick.AddListener(OnBuildClicked);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (buildButton != null)
            buildButton.onClick.RemoveListener(OnBuildClicked);
    }

    /// <summary>
    /// Показывает панель для выбранного слота строительства.
    /// </summary>
    public void Show(ConstructionSlot slot)
    {
        Debug.Log("ConstructionPanel.Show called", this);

        if (slot == null)
            return;

        currentSlot = slot;
        selectedConfig = null;

        BuildOptions(slot.AvailableBuildings);

        if (slot.AvailableBuildings != null && slot.AvailableBuildings.Count > 0)
            SelectConfig(slot.AvailableBuildings[0]);
        else
            RefreshMainInfo();

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Скрывает панель и очищает runtime-данные.
    /// </summary>
    public void Hide()
    {
        currentSlot = null;
        selectedConfig = null;

        ClearOptions();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Создаёт нижний список доступных зданий.
    /// </summary>
    private void BuildOptions(IReadOnlyList<BuildingConfig> configs)
    {
        ClearOptions();

        if (configs == null)
        {
            Debug.LogWarning("ConstructionPanel: configs == null", this);
            return;
        }

        Debug.Log($"ConstructionPanel: building options count = {configs.Count}", this);

        foreach (BuildingConfig config in configs)
        {
            if (config == null)
                continue;

            if (contentRoot == null)
            {
                Debug.LogError("ConstructionPanel: contentRoot is not assigned", this);
                return;
            }

            if (optionPrefab == null)
            {
                Debug.LogError("ConstructionPanel: optionPrefab is not assigned", this);
                return;
            }

            ConstructionOptionItem item = Instantiate(optionPrefab, contentRoot);
            item.Bind(config, SelectConfig);
            optionItems.Add(item);
        }
    }

    /// <summary>
    /// Выбирает здание для просмотра и постройки.
    /// </summary>
    private void SelectConfig(BuildingConfig config)
    {
        selectedConfig = config;
        RefreshMainInfo();
        RefreshSelectionVisual();
        RefreshBuildButton();
    }

    /// <summary>
    /// Обновляет большой информационный блок.
    /// </summary>
    private void RefreshMainInfo()
    {
        if (selectedConfig == null)
        {
            if (titleText != null)
                titleText.text = "Нет здания";

            if (descriptionText != null)
                descriptionText.text = string.Empty;

            if (costText != null)
                costText.text = string.Empty;

            if (buildTimeText != null)
                buildTimeText.text = string.Empty;

            if (previewImage != null)
                previewImage.sprite = null;

            return;
        }

        if (titleText != null)
            titleText.text = selectedConfig.DisplayName;

        if (descriptionText != null)
            descriptionText.text = selectedConfig.Description;

        if (costText != null)
            costText.text = $"Стоимость: {selectedConfig.woodCost} дерева и {selectedConfig.goldCost} золота";

        if (buildTimeText != null)
            buildTimeText.text = $"Время строительства: {selectedConfig.buildTime:0.#} сек";

        if (previewImage != null)
            previewImage.sprite = selectedConfig.Icon;
    }

    /// <summary>
    /// Обновляет подсветку выбранной иконки в нижнем списке.
    /// </summary>
    private void RefreshSelectionVisual()
    {
        foreach (ConstructionOptionItem item in optionItems)
        {
            if (item == null)
                continue;

            item.SetSelected(item.Config == selectedConfig);
        }
    }

    /// <summary>
    /// Обновляет кнопку строительства.
    /// </summary>
    private void RefreshBuildButton()
    {
        if (buildButton == null)
            return;

        bool canBuild = currentSlot != null &&
                        selectedConfig != null &&
                        ResourceStorage.Instance != null &&
                        ResourceStorage.Instance.HasResources(selectedConfig.woodCost, selectedConfig.goldCost);

        buildButton.interactable = canBuild;

        if (buildButtonText != null)
            buildButtonText.text = canBuild ? "Построить" : "Не хватает ресурсов";
    }

    /// <summary>
    /// Запускает строительство выбранного здания.
    /// </summary>
    private void OnBuildClicked()
    {
        if (currentSlot == null || selectedConfig == null)
            return;

        currentSlot.StartConstruction(selectedConfig);
        Hide();
    }

    /// <summary>
    /// Очищает нижний список иконок.
    /// </summary>
    private void ClearOptions()
    {
        foreach (ConstructionOptionItem item in optionItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        optionItems.Clear();
    }
}