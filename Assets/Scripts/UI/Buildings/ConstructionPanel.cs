using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Панель выбора и постройки здания на слоте.
/// </summary>
public sealed class ConstructionPanel : ValidatedMonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _buildTimeText;
    [SerializeField] private Image _previewImage;
    [SerializeField] private Button _buildButton;
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private ConstructionOptionItem _optionPrefab;
    [SerializeField] private SimplePanelTween _panelTween;

    private readonly List<ConstructionOptionItem> _optionItems = new();

    private ResourceStorage _resourceStorage;
    private TechTreeBonusService _techTreeBonusService;
    private BuildingRegistry _buildingRegistry;
    private ConstructionSlot _currentSlot;
    private BuildingConfig _selectedConfig;
    private DiContainer _container;
    private BuildingConfig _tutorialAllowedBuilding;

    public event Action<BuildingConfig> ConstructionStarted;

    public RectTransform PanelRect => _root.transform as RectTransform;

    public SimplePanelTween PanelTween => _panelTween;

    public bool IsVisible => _panelTween.IsVisible;

    [Inject]
    private void Construct(
        ResourceStorage resourceStorage,
        TechTreeBonusService techTreeBonusService,
        BuildingRegistry buildingRegistry,
        DiContainer container)
    {
        _resourceStorage = resourceStorage;
        _techTreeBonusService = techTreeBonusService;
        _buildingRegistry = buildingRegistry;
        _container = container;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        _buildButton.onClick.AddListener(OnBuildClicked);
        _resourceStorage.ResourcesChanged += Refresh;
    }

    private void OnDisable()
    {
        _buildButton.onClick.RemoveListener(OnBuildClicked);
        _resourceStorage.ResourcesChanged -= Refresh;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _titleText, nameof(_titleText));
        valid &= ValidationUtility.IsAssigned(this, _descriptionText, nameof(_descriptionText));
        valid &= ValidationUtility.IsAssigned(this, _costText, nameof(_costText));
        valid &= ValidationUtility.IsAssigned(this, _buildTimeText, nameof(_buildTimeText));
        valid &= ValidationUtility.IsAssigned(this, _previewImage, nameof(_previewImage));
        valid &= ValidationUtility.IsAssigned(this, _buildButton, nameof(_buildButton));
        valid &= ValidationUtility.IsAssigned(this, _contentRoot, nameof(_contentRoot));
        valid &= ValidationUtility.IsAssigned(this, _optionPrefab, nameof(_optionPrefab));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));

        return valid;
    }

    public void Show(ConstructionSlot slot)
    {
        if (slot == null)
        {
            Hide();
            return;
        }

        _currentSlot = slot;
        _selectedConfig = null;

        BuildOptions(slot.AvailableBuildings);

        if (_tutorialAllowedBuilding != null)
            SelectConfig(_tutorialAllowedBuilding);
        else
            SelectFirstAvailableConfig(slot.AvailableBuildings);

        Refresh();
    }

    /// <summary>
    /// Ограничивает выбор здания на шаге обучения.
    /// </summary>
    public void SetTutorialAllowedBuilding(BuildingConfig allowedBuilding)
    {
        _tutorialAllowedBuilding = allowedBuilding;
        Refresh();
    }

    public bool AllowsBuilding(BuildingConfig buildingConfig)
    {
        if (_tutorialAllowedBuilding == null)
            return true;

        return BuildingConfigUtility.Matches(_tutorialAllowedBuilding, buildingConfig);
    }

    public RectTransform GetOptionRect(BuildingConfig config)
    {
        for (int i = 0; i < _optionItems.Count; i++)
        {
            ConstructionOptionItem item = _optionItems[i];

            if (BuildingConfigUtility.Matches(config, item.Config))
                return item.RectTransform;
        }

        return null;
    }

    public void Hide()
    {
        ClearState();
    }

    public void Dismiss()
    {
        ClearState();
        _panelTween.Hide();
    }

    private void ClearState()
    {
        _currentSlot = null;
        _selectedConfig = null;
        _tutorialAllowedBuilding = null;

        ClearOptions();
        ClearInfo();
    }

    private void BuildOptions(IReadOnlyList<BuildingConfig> configs)
    {
        ClearOptions();

        for (int i = 0; i < configs.Count; i++)
        {
            BuildingConfig config = configs[i];

            ConstructionOptionItem item = CreateItem();
            item.Bind(config, SelectConfig, AllowsBuilding);

            _optionItems.Add(item);
        }
    }

    private ConstructionOptionItem CreateItem()
    {
        return _container.InstantiatePrefabForComponent<ConstructionOptionItem>(
            _optionPrefab,
            _contentRoot);
    }

    private void SelectFirstAvailableConfig(IReadOnlyList<BuildingConfig> configs)
    {
        if (configs.Count == 0)
            return;

        SelectConfig(configs[0]);
    }

    private void SelectConfig(BuildingConfig config)
    {
        if (!AllowsBuilding(config))
            return;

        _selectedConfig = config;
        Refresh();
    }

    private void Refresh()
    {
        RefreshSelectedInfo();
        RefreshSelectionVisual();
        RefreshBuildButton();
    }

    private void RefreshSelectedInfo()
    {
        if (_selectedConfig == null)
        {
            ClearInfo();
            return;
        }

        _titleText.text = _selectedConfig.DisplayName;
        _descriptionText.text = _selectedConfig.Description;
        _buildTimeText.text = $"Постройка: {GetBuildTime(_selectedConfig):0.#} секунд";

        RefreshCostText();

        _previewImage.sprite = _selectedConfig.Icon;
    }

    private void RefreshCostText()
    {
        if (_selectedConfig == null)
        {
            _costText.text = "Стоимость: -";
            return;
        }

        string limitText = BuildLimitText(_selectedConfig);
        string blockReason = _currentSlot != null
            ? _currentSlot.GetBuildBlockReason(_selectedConfig)
            : string.Empty;

        string resourceText =
            $"Стоимость\n" +
            $"Wood: {_resourceStorage.Wood}/{_selectedConfig.WoodCost}\n" +
            $"Gold: {_resourceStorage.Gold}/{_selectedConfig.GoldCost}";

        if (!string.IsNullOrEmpty(limitText))
            resourceText = limitText + "\n" + resourceText;

        if (!string.IsNullOrEmpty(blockReason))
            resourceText += "\n" + blockReason;

        _costText.text = resourceText;
    }
    private string BuildLimitText(BuildingConfig config)
    {
        if (!config.UniqueBuilding)
            return string.Empty;

        int currentCount = _buildingRegistry.GetBuiltOrConstructingCount(config);
        int allowedCount = _buildingRegistry.GetAllowedCount(config);

        return $"Лимит: {currentCount}/{allowedCount}";
    }

    private void RefreshBuildButton()
    {
        string blockReason = _currentSlot != null
            ? _currentSlot.GetBuildBlockReason(_selectedConfig)
            : "Слот не выбран";

        _buildButton.interactable = string.IsNullOrEmpty(blockReason);
    }

    private void RefreshSelectionVisual()
    {
        for (int i = 0; i < _optionItems.Count; i++)
        {
            ConstructionOptionItem item = _optionItems[i];
            item.SetSelected(item.Config == _selectedConfig);
            item.RefreshInteractable();
        }
    }
    private float GetBuildTime(BuildingConfig config)
    {
        float buildTime = _techTreeBonusService.ApplyPercentReduction(
            config.BuildTime,
            TechTreeBonusType.BuildAll);

        return Mathf.Max(0.1f, buildTime);
    }

    private void OnBuildClicked()
    {
        if (_currentSlot == null || _selectedConfig == null)
            return;

        bool started = _currentSlot.StartConstruction(_selectedConfig);

        if (!started)
        {
            Refresh();
            return;
        }

        ConstructionStarted?.Invoke(_selectedConfig);
        Dismiss();
    }

    private void ClearInfo()
    {
        _titleText.text = "Здание не выбрано";
        _descriptionText.text = string.Empty;
        _buildTimeText.text = "Постройка: -";
        _costText.text = "Стоимость: -";
        _previewImage.sprite = null;
        _buildButton.interactable = false;
    }

    private void ClearOptions()
    {
        for (int i = 0; i < _optionItems.Count; i++)
            Destroy(_optionItems[i].gameObject);

        _optionItems.Clear();
    }
}
