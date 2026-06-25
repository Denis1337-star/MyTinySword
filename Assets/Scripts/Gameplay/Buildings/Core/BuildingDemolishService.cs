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
    private BuildingBase _pendingBuilding;

    [Inject]
    private void Construct(SelectionSystem selectionSystem)
    {
        _selectionSystem = selectionSystem;
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
            DefaultDemolishMessage,
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

        if (!building.TryDemolishByButton())
        {
            _confirmPanel.Hide();
            return;
        }

        _selectionSystem.ClearSelection();
        _confirmPanel.Hide();
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