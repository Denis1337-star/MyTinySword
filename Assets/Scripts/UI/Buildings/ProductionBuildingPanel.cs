using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Панель производственного здания: найм юнитов и снос.
/// </summary>
public sealed class ProductionBuildingPanel : ValidatedMonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _unitNameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _statsText;
    [SerializeField] private TMP_Text _queueText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Button _hireButton;
    [SerializeField] private Button _demolishButton;
    [SerializeField] private Image _iconImage;
    [SerializeField] private SimplePanelTween _panelTween;

    private readonly EntityEventSubscription<ProductionBuildingBase> _buildingEvents = new();

    private ProductionBuildingBase _currentBuilding;
    private ArmyUnitRegistry _armyUnitRegistry;
    private ResourceStorage _resourceStorage;
    private BuildingDemolishService _buildingDemolishService;

    public RectTransform HireButtonRect => _hireButton.transform as RectTransform;

    public RectTransform PanelRect => _root.transform as RectTransform;

    public SimplePanelTween PanelTween => _panelTween;

    public event Action UnitHired;

    [Inject]
    private void Construct(
        ArmyUnitRegistry armyUnitRegistry,
        ResourceStorage resourceStorage,
        BuildingDemolishService buildingDemolishService)
    {
        _armyUnitRegistry = armyUnitRegistry;
        _resourceStorage = resourceStorage;
        _buildingDemolishService = buildingDemolishService;
    }

    protected override void Awake()
    {
        base.Awake();
        ClearText();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _unitNameText, nameof(_unitNameText));
        valid &= ValidationUtility.IsAssigned(this, _descriptionText, nameof(_descriptionText));
        valid &= ValidationUtility.IsAssigned(this, _statsText, nameof(_statsText));
        valid &= ValidationUtility.IsAssigned(this, _queueText, nameof(_queueText));
        valid &= ValidationUtility.IsAssigned(this, _costText, nameof(_costText));
        valid &= ValidationUtility.IsAssigned(this, _hireButton, nameof(_hireButton));
        valid &= ValidationUtility.IsAssigned(this, _demolishButton, nameof(_demolishButton));
        valid &= ValidationUtility.IsAssigned(this, _iconImage, nameof(_iconImage));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));

        return valid;
    }

    private void OnEnable()
    {
        _hireButton.onClick.AddListener(HireUnit);
        _demolishButton.onClick.AddListener(RequestDemolishBuilding);

        _armyUnitRegistry.OnArmyChanged += Refresh;
        _resourceStorage.ResourcesChanged += Refresh;

        BindCurrentBuildingEvents();
        Refresh();
    }

    private void OnDisable()
    {
        _hireButton.onClick.RemoveListener(HireUnit);
        _demolishButton.onClick.RemoveListener(RequestDemolishBuilding);

        _armyUnitRegistry.OnArmyChanged -= Refresh;
        _resourceStorage.ResourcesChanged -= Refresh;

        ClearBuildingSubscription();
    }

    public void Show(ProductionBuildingBase building)
    {
        if (building == null)
        {
            Hide();
            return;
        }

        if (_buildingEvents.IsBoundTo(building))
        {
            Refresh();
            return;
        }

        ClearBuildingSubscription();

        _currentBuilding = building;
        BindCurrentBuildingEvents();
        Refresh();
    }

    public void Hide()
    {
        ClearBuildingSubscription();

        _currentBuilding = null;

        ClearText();
    }

    public void Dismiss()
    {
        Hide();
        _panelTween.Hide();
    }

    private void HireUnit()
    {
        if (!_currentBuilding.TryHireUnit())
            return;

        UnitHired?.Invoke();
        Refresh();
    }

    private void RequestDemolishBuilding()
    {
        _buildingDemolishService.RequestDemolish(_currentBuilding);
    }

    private void Refresh()
    {
        if (_currentBuilding == null)
        {
            ClearText();
            return;
        }

        UnitConfig config = _currentBuilding.UnitConfig;

        _unitNameText.text = config.DisplayName;
        _descriptionText.text = config.Description;
        _statsText.text = config.GetPreviewStatsText();
        _queueText.text = BuildQueueText(
            _currentBuilding.QueueCount,
            _currentBuilding.MaxQueue,
            _armyUnitRegistry.CommittedPlayerArmySlots,
            _armyUnitRegistry.MaxPlayerArmyUnits,
            _currentBuilding.CurrentBuildTime);

        string blockReason = _currentBuilding.GetHireBlockReason();

        _costText.text = BuildCostText(
            _currentBuilding.CurrentWoodCost,
            _currentBuilding.CurrentMeatCost,
            blockReason);

        _hireButton.interactable = _currentBuilding.CanEnqueue();
        BuildingDemolishRules.RefreshButton(_demolishButton, _currentBuilding);

        _iconImage.sprite = config.Icon;
    }

    private void ClearText()
    {
        _unitNameText.text = string.Empty;
        _descriptionText.text = string.Empty;
        _statsText.text = string.Empty;
        _queueText.text = BuildQueueText(0, 0, 0, 0, 0f);
        _costText.text = "Стоимость: -";

        _hireButton.interactable = false;
        _demolishButton.gameObject.SetActive(false);

        _iconImage.sprite = null;
    }

    private static string BuildQueueText(
     int queueCount,
     int maxQueue,
     int armySlots,
     int maxArmySlots,
     float buildTime)
    {
        return
            $"В очереди: {queueCount}/{maxQueue}  Армия: {armySlots}/{maxArmySlots}\n" +
            $"Обучение: {buildTime:0.#} сек.";
    }
    private static string BuildCostText(int woodCost, int meatCost, string blockReason)
    {
        string text = $"Стоимость: дерево {woodCost} / мясо {meatCost}";

        if (!string.IsNullOrEmpty(blockReason))
            text += "\n" + blockReason;

        return text;
    }

    private void BindCurrentBuildingEvents()
    {
        _buildingEvents.Bind(
            _currentBuilding,
            b => b.OnQueueChanged += Refresh,
            b => b.OnQueueChanged -= Refresh);
    }

    private void ClearBuildingSubscription()
    {
        _buildingEvents.Clear(b => b.OnQueueChanged -= Refresh);
    }
}
