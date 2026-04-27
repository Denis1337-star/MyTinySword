using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
///  показывает подходящую панель
/// </summary>
public class SelectionUiPresenter : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private WorkerCommandPanel _workerCommandPanel;
    private HousePanel _housePanel;
    private ProductionBuildingPanel _productionBuildingPanel;
    private ArmySelectionPanel _armySelectionPanel;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        WorkerCommandPanel workerCommandPanel,
        HousePanel housePanel,
        ProductionBuildingPanel productionBuildingPanel,
        ArmySelectionPanel armySelectionPanel)
    {
        _selectionSystem = selectionSystem;
        _workerCommandPanel = workerCommandPanel;
        _housePanel = housePanel;
        _productionBuildingPanel = productionBuildingPanel;
        _armySelectionPanel = armySelectionPanel;
    }

    private void OnEnable()
    {
        if (_selectionSystem == null)
            return;

        _selectionSystem.SelectionChanged += OnSelectionChanged;
        _selectionSystem.SelectionCleared += OnSelectionCleared;
    }

    private void OnDisable()
    {
        if (_selectionSystem == null)
            return;

        _selectionSystem.SelectionChanged -= OnSelectionChanged;
        _selectionSystem.SelectionCleared -= OnSelectionCleared;
    }

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        HideAll();

        if (_selectionSystem == null)
            return;

        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.GetSelectedUnits();

        if (ContainsPlayerArmyUnits(selectedUnits))
        {
            _armySelectionPanel?.Show(selectedUnits);
            return;
        }

        if (selectable == null)
            return;

        Worker worker = selectable.GetComponent<Worker>();
        if (worker == null)
            worker = selectable.GetComponentInParent<Worker>();

        if (worker != null)
        {
            _workerCommandPanel?.ShowForWorker(worker);
            return;
        }

        House house = selectable.GetComponent<House>();
        if (house == null)
            house = selectable.GetComponentInParent<House>();

        if (house != null)
        {
            _housePanel?.Show(house);
            return;
        }

        ProductionBuildingBase productionBuilding = selectable.GetComponent<ProductionBuildingBase>();
        if (productionBuilding == null)
            productionBuilding = selectable.GetComponentInParent<ProductionBuildingBase>();

        if (productionBuilding != null)
            _productionBuildingPanel?.Show(productionBuilding);
    }

    private void OnSelectionCleared()
    {
        HideAll();
    }

    private void HideAll()
    {
        _workerCommandPanel?.Hide();
        _housePanel?.Hide();
        _productionBuildingPanel?.Hide();
        _armySelectionPanel?.Hide();
    }

    private bool ContainsPlayerArmyUnits(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        if (selectedUnits == null)
            return false;

        foreach (UnitSelectable selectable in selectedUnits)
        {
            if (selectable == null)
                continue;

            ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();
            if (armyUnit == null)
                armyUnit = selectable.GetComponentInParent<ArmyUnit>();

            if (armyUnit != null && armyUnit.IsPlayerUnit())
                return true;
        }

        return false;
    }
}