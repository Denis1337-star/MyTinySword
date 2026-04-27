using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

/// <summary>
/// Панель строительства
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
    private ResourceStorage resourceStorage;

    [Inject]
    private void Construct(ResourceStorage resourceStorage)
    {
        this.resourceStorage = resourceStorage;
    }

    private void Awake()
    {
        if (buildButton != null)
            buildButton.onClick.AddListener(OnBuildClicked);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        resourceStorage.ResourcesChanged
            .Subscribe(_ => RefreshBuildButton())
            .AddTo(this);
    }

    private void OnDestroy()
    {
        if (buildButton != null)
            buildButton.onClick.RemoveListener(OnBuildClicked);
    }

    public void Show(ConstructionSlot slot)
    {
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
        RefreshBuildButton();
    }

    public void Hide()
    {
        currentSlot = null;
        selectedConfig = null;

        ClearOptions();
        gameObject.SetActive(false);
    }

    private void BuildOptions(IReadOnlyList<BuildingConfig> configs)
    {
        ClearOptions();

        if (configs == null)
            return;

        foreach (BuildingConfig config in configs)
        {
            if (config == null)
                continue;

            if (contentRoot == null || optionPrefab == null)
                return;

            ConstructionOptionItem item = Instantiate(optionPrefab, contentRoot);
            item.Bind(config, SelectConfig);
            optionItems.Add(item);
        }
    }

    private void SelectConfig(BuildingConfig config)
    {
        selectedConfig = config;

        RefreshMainInfo();
        RefreshSelectionVisual();
        RefreshBuildButton();
    }

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
        {
            int currentWood = resourceStorage != null ? resourceStorage.Wood : 0;
            int currentGold = resourceStorage != null ? resourceStorage.Gold : 0;

            costText.text =
                $"Стоимость строительства\n" +
                $"Дерево: {currentWood} / {selectedConfig.WoodCost}\n" +
                $"Золото: {currentGold} / {selectedConfig.GoldCost}";
        }

        if (buildTimeText != null)
            buildTimeText.text = $"Время строительства: {selectedConfig.BuildTime:0.#} сек.";

        if (previewImage != null)
            previewImage.sprite = selectedConfig.Icon;
    }

    private void RefreshSelectionVisual()
    {
        foreach (ConstructionOptionItem item in optionItems)
        {
            if (item == null)
                continue;

            item.SetSelected(item.Config == selectedConfig);
        }
    }

    private void RefreshBuildButton()
    {
        if (buildButton == null)
            return;

        string blockReason = currentSlot != null
            ? currentSlot.GetBuildBlockReason(selectedConfig)
            : "Слот не выбран";

        bool canBuild = string.IsNullOrEmpty(blockReason);

        buildButton.interactable = canBuild;

        if (buildButtonText != null)
            buildButtonText.text = canBuild ? "Построить" : blockReason;

        RefreshMainInfo();
    }

    private void OnBuildClicked()
    {
        if (currentSlot == null || selectedConfig == null)
            return;

        bool started = currentSlot.StartConstruction(selectedConfig);
        if (!started)
        {
            RefreshBuildButton();
            return;
        }

        Hide();
    }

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