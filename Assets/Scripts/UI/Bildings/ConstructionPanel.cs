using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

/// <summary>
/// UI-панель строительства
/// Показывает список зданий, доступных для выбранного ConstructionSlot
/// </summary>
public class ConstructionPanel : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Selected Building Info")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _bildTimeText;

    [Header("Preview")]
    [SerializeField] private Image _previewImage;

    [Header("Build Button")]
    [SerializeField] private Button _buildButton;

    [Header("Options List")]
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private ConstructionOptionItem _optionPrefab;

    private readonly List<ConstructionOptionItem> _optionItems = new();

    private ResourceStorage _resourceStorage;
    private ConstructionSlot _currentSlot;
    private BuildingConfig _selectedConfig;

    [Inject]
    private void Construct(ResourceStorage resourceStorage)
    {
        _resourceStorage = resourceStorage;
    }

    private void Awake()
    {
        _buildButton?.onClick.AddListener(OnBuildClicked);
        Hide();
    }

    private void Start()
    {
        _resourceStorage.ResourcesChanged
            .Subscribe(_ => Refresh())
            .AddTo(this);
    }

    private void OnDestroy()
    {
        _buildButton?.onClick.RemoveListener(OnBuildClicked);
    }

    /// <summary>
    /// Показывает панель для выбранного строительного слота
    /// </summary>
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

        if (slot.AvailableBuildings != null && slot.AvailableBuildings.Count > 0)
            SelectConfig(slot.AvailableBuildings[0]);
        else
            Refresh();

        ShowRoot();
    }

    /// <summary>
    /// Скрывает панель и очищает выбранный слот.
    /// </summary>
    public void Hide()
    {
        _currentSlot = null;
        _selectedConfig = null;

        ClearOptions();
        ClearInfo();

        if (_root != null)
            _root.SetActive(false);
        else
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

            ConstructionOptionItem item = CreateItem();
            if (item == null)
                continue;

            item.Bind(config, SelectConfig);
            _optionItems.Add(item);
        }
    }

    private ConstructionOptionItem CreateItem()
    {
        if (_contentRoot == null || _optionPrefab == null)
            return null;

        return Instantiate(_optionPrefab, _contentRoot);
    }

    private void SelectConfig(BuildingConfig config)
    {
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

        if (_titleText != null)
            _titleText.text = _selectedConfig.DisplayName;

        if (_descriptionText != null)
            _descriptionText.text = _selectedConfig.Description;
        
        if(_bildTimeText != null)
            _bildTimeText.text = $"Строится: {_selectedConfig.BuildTime} секунд";

        if (_costText != null)
        {
            int currentWood = _resourceStorage != null ? _resourceStorage.Wood : 0;
            int currentGold = _resourceStorage != null ? _resourceStorage.Gold : 0;

            _costText.text =
                $"Стоимость\n" +
                $"Wood: {currentWood}/{_selectedConfig.WoodCost}\n" +
                $"Gold: {currentGold}/{_selectedConfig.GoldCost}";
        }

        if (_previewImage != null)
            _previewImage.sprite = _selectedConfig.Icon;
    }

    private void RefreshBuildButton()
    {
        if (_buildButton == null)
            return;

        string blockReason = _currentSlot != null
            ? _currentSlot.GetBuildBlockReason(_selectedConfig)
            : "Слот не выбран";

        bool canBuild = string.IsNullOrEmpty(blockReason);

        _buildButton.interactable = canBuild;
    }

    private void RefreshSelectionVisual()
    {
        foreach (ConstructionOptionItem item in _optionItems)
        {
            if (item == null)
                continue;

            item.SetSelected(item.Config == _selectedConfig);
        }
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

        Hide();
    }

    private void ClearInfo()
    {
        if (_titleText != null)
            _titleText.text = "Здание не выбрано";

        if (_descriptionText != null)
            _descriptionText.text = string.Empty;

        if (_costText != null)
            _costText.text = "Стоимость: -";

        if (_previewImage != null)
            _previewImage.sprite = null;

        if (_buildButton != null)
            _buildButton.interactable = false;
    }

    private void ClearOptions()
    {
        foreach (ConstructionOptionItem item in _optionItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        _optionItems.Clear();
    }

    private void ShowRoot()
    {
        if (_root != null)
            _root.SetActive(true);
        else
            gameObject.SetActive(true);
    }
}