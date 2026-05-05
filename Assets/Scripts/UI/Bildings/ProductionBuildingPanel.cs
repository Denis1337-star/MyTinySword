using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Общая UI-панель производственных зданий.
/// </summary>
public sealed class ProductionBuildingPanel : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Texts")]
    [SerializeField] private TMP_Text _unitNameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _statsText;
    [SerializeField] private TMP_Text _queueText;
    [SerializeField] private TMP_Text _costText;

    [Header("Controls")]
    [SerializeField] private Button _hireButton;
    [SerializeField] private Image _iconImage;

    private ProductionBuildingBase _currentBuilding;
    private ArmyUnitRegistry _armyUnitRegistry;

    private bool _subscribedToBuilding;
    private bool _subscribedToRegistry;

    [Inject]
    private void Construct(ArmyUnitRegistry armyUnitRegistry)
    {
        _armyUnitRegistry = armyUnitRegistry;
    }

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        _hireButton?.onClick.AddListener(HireUnit);

        SubscribeToBuilding();
        SubscribeToRegistry();

        Refresh();
    }

    private void OnDisable()
    {
        _hireButton?.onClick.RemoveListener(HireUnit);

        UnsubscribeFromBuilding();
        UnsubscribeFromRegistry();
    }

    public void Show(ProductionBuildingBase building)
    {
        if (building == null)
        {
            Hide();
            return;
        }

        if (_currentBuilding == building)
        {
            ShowRoot();
            Refresh();
            return;
        }

        UnsubscribeFromBuilding();

        _currentBuilding = building;

        SubscribeToBuilding();

        ShowRoot();
        Refresh();
    }

    public void Hide()
    {
        UnsubscribeFromBuilding();

        _currentBuilding = null;

        ClearText();

        if (_root != null)
            _root.SetActive(false);
    }

    private void HireUnit()
    {
        if (_currentBuilding == null)
            return;

        _currentBuilding.TryHireUnit();
        Refresh();
    }

    private void Refresh()
    {
        if (_currentBuilding == null)
        {
            ClearText();
            return;
        }

        UnitConfig config = _currentBuilding.UnitConfig;
        if (config == null)
        {
            ClearText();
            return;
        }

        if (_unitNameText != null)
            _unitNameText.text = config.DisplayName;

        if (_descriptionText != null)
            _descriptionText.text = config.Description;

        if (_statsText != null)
            _statsText.text = config.GetPreviewStatsText();

        if (_queueText != null)
        {
            string armyLimitText = _armyUnitRegistry != null
                ? $"Армия: {_armyUnitRegistry.CommittedPlayerArmySlots}/{_armyUnitRegistry.MaxPlayerArmyUnits}"
                : "Армия: -";

            _queueText.text =
                $"В очереди: {_currentBuilding.QueueCount}/{_currentBuilding.MaxQueue}\n" +
                armyLimitText;
        }

        if (_costText != null)
        {
            string blockReason = _currentBuilding.GetHireBlockReason();

            _costText.text =
                $"Стоимость: Wood {config.WoodCost} / Meat {config.MeatCost}";

            if (!string.IsNullOrEmpty(blockReason))
                _costText.text += $"\n{blockReason}";
        }

        if (_hireButton != null)
            _hireButton.interactable = _currentBuilding.CanEnqueue();

        if (_iconImage != null)
            _iconImage.sprite = config.Icon;
    }

    private void ClearText()
    {
        if (_unitNameText != null)
            _unitNameText.text = string.Empty;

        if (_descriptionText != null)
            _descriptionText.text = string.Empty;

        if (_statsText != null)
            _statsText.text = string.Empty;

        if (_queueText != null)
            _queueText.text = "В очереди: 0";

        if (_costText != null)
            _costText.text = "Стоимость: -";

        if (_hireButton != null)
            _hireButton.interactable = false;

        if (_iconImage != null)
            _iconImage.sprite = null;
    }

    private void SubscribeToBuilding()
    {
        if (_subscribedToBuilding)
            return;

        if (_currentBuilding == null)
            return;

        _currentBuilding.OnQueueChanged += Refresh;

        _subscribedToBuilding = true;
    }

    private void UnsubscribeFromBuilding()
    {
        if (!_subscribedToBuilding)
            return;

        if (_currentBuilding != null)
            _currentBuilding.OnQueueChanged -= Refresh;

        _subscribedToBuilding = false;
    }

    private void SubscribeToRegistry()
    {
        if (_subscribedToRegistry)
            return;

        if (_armyUnitRegistry == null)
            return;

        _armyUnitRegistry.OnArmyChanged += Refresh;

        _subscribedToRegistry = true;
    }

    private void UnsubscribeFromRegistry()
    {
        if (!_subscribedToRegistry)
            return;

        if (_armyUnitRegistry != null)
            _armyUnitRegistry.OnArmyChanged -= Refresh;

        _subscribedToRegistry = false;
    }

    private void ShowRoot()
    {
        if (_root != null)
            _root.SetActive(true);
    }
}