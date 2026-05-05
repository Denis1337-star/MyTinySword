using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Показывает UI-панель в зависимости от выбранного объекта
/// </summary>
public sealed class SelectionUiPresenter : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private WorkerCommandPanel _workerCommandPanel;
    private HousePanel _housePanel;
    private ProductionBuildingPanel _productionBuildingPanel;
    private ArmySelectionPanel _armySelectionPanel;

    private bool _isSubscribed;

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

    private void Start()
    {
        Subscribe();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        HideAll();

        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

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

    private void Subscribe()
    {
        if (_isSubscribed)
            return;

        if (_selectionSystem == null)
            return;

        _selectionSystem.SelectionChanged += OnSelectionChanged;
        _selectionSystem.SelectionCleared += OnSelectionCleared;

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
            return;

        if (_selectionSystem != null)
        {
            _selectionSystem.SelectionChanged -= OnSelectionChanged;
            _selectionSystem.SelectionCleared -= OnSelectionCleared;
        }

        _isSubscribed = false;
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

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            UnitSelectable selectable = selectedUnits[i];
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