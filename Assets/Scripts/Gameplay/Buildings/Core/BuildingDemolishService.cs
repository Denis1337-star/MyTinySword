using UnityEngine;
using Zenject;

/// <summary>
/// Запрос сноса здания через общую confirm-панель.
/// </summary>
public sealed class BuildingDemolishService : ValidatedMonoBehaviour
{
    private const string CannotDemolishMessage = "Это здание нельзя снести.";
    private const string TutorialBlockedMessage = "Сейчас обучение не разрешает снести здание.";
    private const string DefaultDemolishMessage =
        "Вы уверены, что хотите снести здание?\nРесурсы не будут возвращены.";

    [SerializeField] private BuildingDemolishConfirmPanel _confirmPanel;

    private SelectionSystem _selectionSystem;
    private ResourceStorage _resourceStorage;
    private TechTreeBonusService _techTreeBonusService;

    private BuildingBase _pendingBuilding;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        ResourceStorage resourceStorage,
        TechTreeBonusService techTreeBonusService)
    {
        _selectionSystem = selectionSystem;
        _resourceStorage = resourceStorage;
        _techTreeBonusService = techTreeBonusService;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _confirmPanel, nameof(_confirmPanel));

        return valid;
    }

    public void RequestDemolish(BuildingBase building)
    {
        _pendingBuilding = building;

        if (building == null)
        {
            ShowBlocked(CannotDemolishMessage);
            return;
        }

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