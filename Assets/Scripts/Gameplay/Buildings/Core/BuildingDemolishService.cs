using UnityEngine;
using Zenject;

/// <summary>
/// Запрос сноса здания через confirm-панель.
/// </summary>
public sealed class BuildingDemolishService : ValidatedMonoBehaviour
{
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
        if (!BuildingDemolishRules.CanDemolish(building))
            return;

        _pendingBuilding = building;
        _confirmPanel.Show(ConfirmDemolish, CancelDemolish);
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
}
