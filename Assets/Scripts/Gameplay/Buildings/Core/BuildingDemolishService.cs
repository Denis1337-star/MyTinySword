using UnityEngine;

/// <summary>
/// Запрос сноса здания через общую confirm-панель.
/// </summary>
public sealed class BuildingDemolishService
{
    private readonly SelectionSystem _selectionSystem;
    private readonly ResourceStorage _resourceStorage;
    private readonly TechTreeBonusService _techTreeBonusService;
    private readonly BuildingDemolishConfirmPanel _confirmPanel;

    private BuildingBase _pendingBuilding;

    public BuildingDemolishService(
        SelectionSystem selectionSystem,
        ResourceStorage resourceStorage,
        TechTreeBonusService techTreeBonusService,
        BuildingDemolishConfirmPanel confirmPanel)
    {
        _selectionSystem = selectionSystem;
        _resourceStorage = resourceStorage;
        _techTreeBonusService = techTreeBonusService;
        _confirmPanel = confirmPanel;
    }

    public void RequestDemolish(BuildingBase building)
    {
        _pendingBuilding = building;

        if (!building.CanBeDemolishedByButton)
        {
            ShowBlocked(() => GameUiText.CannotDemolishBuilding);
            return;
        }

        if (!TutorialInputGuard.AllowsDemolishBuilding())
        {
            ShowBlocked(() => GameUiText.TutorialDemolishBlocked);
            return;
        }

        _confirmPanel.ShowAllowed(
            BuildDemolishMessage,
            ConfirmDemolish,
            CancelDemolish);
    }

    private void ConfirmDemolish()
    {
        BuildingBase building = _pendingBuilding;
        _pendingBuilding = null;

        if (building == null)
        {
            _confirmPanel.Hide();
            return;
        }

        int woodRefund = GetRefundAmount(building.Config.WoodCost);
        int goldRefund = GetRefundAmount(building.Config.GoldCost);

        if (!building.TryDemolishByButton())
        {
            _confirmPanel.Hide();
            return;
        }

        AddRefund(ResourceType.Wood, woodRefund);
        AddRefund(ResourceType.Gold, goldRefund);

        _selectionSystem.ClearSelection();
        _confirmPanel.Hide();
    }

    private string BuildDemolishMessage()
    {
        if (_pendingBuilding == null)
            return GameUiText.DemolishConfirmNoRefund;

        int woodRefund = GetRefundAmount(_pendingBuilding.Config.WoodCost);
        int goldRefund = GetRefundAmount(_pendingBuilding.Config.GoldCost);

        if (woodRefund <= 0 && goldRefund <= 0)
            return GameUiText.DemolishConfirmNoRefund;

        return GameUiText.DemolishConfirmWithRefund(woodRefund, goldRefund);
    }

    private int GetRefundAmount(int cost)
    {
        if (cost <= 0)
            return 0;

        float refundPercent = _techTreeBonusService.GetBonusValue(TechTreeBonusType.DemolishRefund);
        float refund = cost * refundPercent / 100f;

        return Mathf.FloorToInt(refund);
    }

    private void AddRefund(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
            return;

        _resourceStorage.AddResource(resourceType, amount);
    }

    private void CancelDemolish()
    {
        _pendingBuilding = null;
        _confirmPanel.Hide();
    }

    private void ShowBlocked(System.Func<string> messageProvider)
    {
        _pendingBuilding = null;
        _confirmPanel.ShowBlocked(messageProvider, CancelDemolish);
    }
}
