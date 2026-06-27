using UnityEngine;

/// <summary>
/// Запрос сноса здания через общую confirm-панель.
/// </summary>
public sealed class BuildingDemolishService
{
    private const string CannotDemolishMessage = "Это здание нельзя снести.";
    private const string TutorialBlockedMessage = "Сейчас обучение не разрешает снести здание.";
    private const string DefaultDemolishMessage =
        "Вы уверены, что хотите снести здание?\nРесурсы не будут возвращены.";

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
            ShowBlocked(CannotDemolishMessage);
            return;
        }

        if (!TutorialInputGuard.AllowsDemolishBuilding())
        {
            ShowBlocked(TutorialBlockedMessage);
            return;
        }

        _confirmPanel.ShowAllowed(
            BuildDemolishMessage(building),
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
    private string BuildDemolishMessage(BuildingBase building)
    {
        int woodRefund = GetRefundAmount(building.Config.WoodCost);
        int goldRefund = GetRefundAmount(building.Config.GoldCost);

        if (woodRefund <= 0 && goldRefund <= 0)
            return DefaultDemolishMessage;

        return
            "Вы уверены, что хотите снести здание?\n" +
            "При сносе будет возвращено:\n" +
            $"Дерево: {woodRefund}\n" +
            $"Золото: {goldRefund}";
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

    private void ShowBlocked(string message)
    {
        _pendingBuilding = null;
        _confirmPanel.ShowBlocked(message, CancelDemolish);
    }
}