using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет отображением UI панелей в зависимости от текущего выбора.
/// Важно: специализированные здания проверяем раньше BuildingBase,
/// потому что House, ProductionBuildingBase и Tower наследуются от BuildingBase.
/// </summary>
public sealed class SelectionUiPresenter : ValidatedMonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private WorkerCommandPanel _workerCommandPanel;
    [SerializeField] private HousePanel _housePanel;
    [SerializeField] private ProductionBuildingPanel _productionBuildingPanel;
    [SerializeField] private BuildingActionPanel _buildingActionPanel;
    [SerializeField] private ArmySelectionPanel _armySelectionPanel;
    [SerializeField] private ConstructionPanel _constructionPanel;

    private SelectionSystem _selectionSystem;
    private bool _isSubscribed;

    [Inject]
    private void Construct(SelectionSystem selectionSystem)
    {
        _selectionSystem = selectionSystem;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        HideAll();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void Start()
    {
        Subscribe();
        RefreshFromCurrentSelection();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _workerCommandPanel, nameof(_workerCommandPanel));
        valid &= ValidationUtility.IsAssigned(this, _housePanel, nameof(_housePanel));
        valid &= ValidationUtility.IsAssigned(this, _productionBuildingPanel, nameof(_productionBuildingPanel));
        valid &= ValidationUtility.IsAssigned(this, _buildingActionPanel, nameof(_buildingActionPanel));
        valid &= ValidationUtility.IsAssigned(this, _armySelectionPanel, nameof(_armySelectionPanel));
        valid &= ValidationUtility.IsAssigned(this, _constructionPanel, nameof(_constructionPanel));

        return valid;
    }

    private void Subscribe()
    {
        if (_isSubscribed)
            return;

        if (_selectionSystem == null)
            return;

        _selectionSystem.SelectionChanged += HandleSelectionChanged;
        _selectionSystem.SelectionCleared += HandleSelectionCleared;

        _isSubscribed = true;

        RefreshFromCurrentSelection();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
            return;

        if (_selectionSystem != null)
        {
            _selectionSystem.SelectionChanged -= HandleSelectionChanged;
            _selectionSystem.SelectionCleared -= HandleSelectionCleared;
        }

        _isSubscribed = false;
    }

    private void RefreshFromCurrentSelection()
    {
        if (_selectionSystem == null)
        {
            HideAll();
            return;
        }

        UnitSelectable currentSelection = _selectionSystem.CurrentSelection;

        if (currentSelection == null)
        {
            HandleSelectionCleared();
            return;
        }

        HandleSelectionChanged(currentSelection);
    }

    private void HandleSelectionChanged(UnitSelectable selectable)
    {
        HideAll();

        if (selectable == null)
            return;

        if (TryShowArmyPanel())
            return;

        if (TryShowWorkerPanel(selectable))
            return;

        if (TryShowHousePanel(selectable))
            return;

        if (TryShowProductionBuildingPanel(selectable))
            return;

        if (TryShowConstructionPanel(selectable))
            return;

        TryShowBuildingActionPanel(selectable);
    }

    private void HandleSelectionCleared()
    {
        HideAll();
    }

    private bool TryShowArmyPanel()
    {
        if (_selectionSystem == null)
            return false;

        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

        if (selectedUnits == null || selectedUnits.Count == 0)
            return false;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            UnitSelectable selectable = selectedUnits[i];

            if (selectable == null)
                continue;

            ArmyUnit armyUnit = FindComponentNearSelectable<ArmyUnit>(selectable);

            if (armyUnit == null)
                continue;

            if (!armyUnit.IsPlayerUnit())
                continue;

            _armySelectionPanel.Show(selectedUnits);
            return true;
        }

        return false;
    }

    private bool TryShowWorkerPanel(UnitSelectable selectable)
    {
        Worker worker = FindComponentNearSelectable<Worker>(selectable);

        if (worker == null)
            return false;

        _workerCommandPanel.ShowForWorker(worker);
        return true;
    }

    private bool TryShowHousePanel(UnitSelectable selectable)
    {
        House house = FindComponentNearSelectable<House>(selectable);

        if (house == null)
            return false;

        _housePanel.Show(house);
        return true;
    }

    private bool TryShowProductionBuildingPanel(UnitSelectable selectable)
    {
        ProductionBuildingBase building = FindComponentNearSelectable<ProductionBuildingBase>(selectable);

        if (building == null)
            return false;

        _productionBuildingPanel.Show(building);
        return true;
    }

    private bool TryShowConstructionPanel(UnitSelectable selectable)
    {
        ConstructionSlot slot = FindComponentNearSelectable<ConstructionSlot>(selectable);

        if (slot == null)
            return false;

        _constructionPanel.Show(slot);
        return true;
    }

    private bool TryShowBuildingActionPanel(UnitSelectable selectable)
    {
        BuildingBase building = FindComponentNearSelectable<BuildingBase>(selectable);

        if (building == null)
            return false;

        _buildingActionPanel.Show(building);
        return true;
    }

    private T FindComponentNearSelectable<T>(UnitSelectable selectable) where T : Component
    {
        if (selectable == null)
            return null;

        T component = selectable.GetComponent<T>();

        if (component != null)
            return component;

        component = selectable.GetComponentInParent<T>();

        if (component != null)
            return component;

        return selectable.GetComponentInChildren<T>();
    }

    private void HideAll()
    {
        _workerCommandPanel.Hide();
        _housePanel.Hide();
        _productionBuildingPanel.Hide();
        _buildingActionPanel.Hide();
        _armySelectionPanel.Hide();
        _constructionPanel.Hide();
    }
}