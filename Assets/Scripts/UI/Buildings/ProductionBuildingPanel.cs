using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI панель производственного здания
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
    [SerializeField] private Image _iconImage;

    private ProductionBuildingBase _currentBuilding;
    private ProductionBuildingBase _subscribedBuilding;
    private ArmyUnitRegistry _armyUnitRegistry;

    [Inject]
    private void Construct(ArmyUnitRegistry armyUnitRegistry)
    {
        _armyUnitRegistry = armyUnitRegistry;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        Hide();
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
        valid &= ValidationUtility.IsAssigned(this, _iconImage, nameof(_iconImage));

        return valid;
    }

    private void OnEnable()
    {
        _hireButton.onClick.AddListener(HireUnit);
        _armyUnitRegistry.OnArmyChanged += Refresh;

        SubscribeToCurrentBuilding();
        Refresh();
    }

    private void OnDisable()
    {
        _hireButton.onClick.RemoveListener(HireUnit);
        _armyUnitRegistry.OnArmyChanged -= Refresh;

        UnsubscribeFromCurrentBuilding();
    }

    public void Show(ProductionBuildingBase building)
    {
        if (building == null)
        {
            Hide();
            return;
        }

        if (_currentBuilding != building)
        {
            UnsubscribeFromCurrentBuilding();

            _currentBuilding = building;

            SubscribeToCurrentBuilding();
        }

        ShowRoot();
        Refresh();
    }

    public void Hide()
    {
        UnsubscribeFromCurrentBuilding();

        _currentBuilding = null;

        ClearText();
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

        _unitNameText.text = config.DisplayName;
        _descriptionText.text = config.Description;
        _statsText.text = config.GetPreviewStatsText();

        _queueText.text =
            $"В очереди: {_currentBuilding.QueueCount}/{_currentBuilding.MaxQueue}\n" +
            $"Армия: {_armyUnitRegistry.CommittedPlayerArmySlots}/{_armyUnitRegistry.MaxPlayerArmyUnits}";

        //string blockReason = _currentBuilding.GetHireBlockReason();

        _costText.text =
            $"Стоимость: дерево {config.WoodCost} / мясо {config.MeatCost}";

        //if (!string.IsNullOrEmpty(blockReason))
        //    _costText.text += $"\n{blockReason}";

        _hireButton.interactable = _currentBuilding.CanEnqueue();
        _iconImage.sprite = config.Icon;
    }

    private void ClearText()
    {
        _unitNameText.text = string.Empty;
        _descriptionText.text = string.Empty;
        _statsText.text = string.Empty;
        _queueText.text = "В очереди: 0";
        _costText.text = "Стоимость: -";

        _hireButton.interactable = false;
        _iconImage.sprite = null;
    }

    private void SubscribeToCurrentBuilding()
    {
        if (_currentBuilding == null)
            return;

        if (_subscribedBuilding == _currentBuilding)
            return;

        _currentBuilding.OnQueueChanged += Refresh;
        _subscribedBuilding = _currentBuilding;
    }

    private void UnsubscribeFromCurrentBuilding()
    {
        if (_subscribedBuilding == null)
            return;

        _subscribedBuilding.OnQueueChanged -= Refresh;
        _subscribedBuilding = null;
    }

    private void ShowRoot()
    {
        _root.SetActive(true);
    }
}