using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜ UI-˜˜˜˜˜˜˜ ˜ ˜˜˜˜˜˜˜˜˜˜˜ ˜˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜.
/// ˜˜˜˜˜˜˜˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜ BuildingBase.
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

        _selectionSystem.SelectionChanged += HandleSelectionChanged;
        _selectionSystem.SelectionCleared += HandleSelectionCleared;

        _isSubscribed = true;

        RefreshFromCurrentSelection();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
            return;

        _selectionSystem.SelectionChanged -= HandleSelectionChanged;
        _selectionSystem.SelectionCleared -= HandleSelectionCleared;

        _isSubscribed = false;
    }

    private void RefreshFromCurrentSelection()
    {
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
        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

        if (!ArmyUnitSelectionUtility.HasAnyPlayerArmyUnit(selectedUnits))
            return false;

        if (!_armySelectionPanel.Show(selectedUnits))
            return false;

        ShowPanelTween(_armySelectionPanel.PanelTween);
        return true;
    }

    private bool TryShowWorkerPanel(UnitSelectable selectable)
    {
        Worker worker = SelectableUtility.FindNear<Worker>(selectable);

        if (worker == null)
            return false;

        _workerCommandPanel.ShowForWorker(worker);
        ShowPanelTween(_workerCommandPanel.PanelTween);
        return true;
    }

    private bool TryShowHousePanel(UnitSelectable selectable)
    {
        House house = SelectableUtility.FindNear<House>(selectable);

        if (house == null)
            return false;

        _housePanel.Show(house);
        ShowPanelTween(_housePanel.PanelTween);
        return true;
    }

    private bool TryShowProductionBuildingPanel(UnitSelectable selectable)
    {
        ProductionBuildingBase building = SelectableUtility.FindNear<ProductionBuildingBase>(selectable);

        if (building == null)
            return false;

        _productionBuildingPanel.Show(building);
        ShowPanelTween(_productionBuildingPanel.PanelTween);
        return true;
    }

    private bool TryShowConstructionPanel(UnitSelectable selectable)
    {
        ConstructionSlot slot = SelectableUtility.FindNear<ConstructionSlot>(selectable);

        if (slot == null)
            return false;

        _constructionPanel.Show(slot);
        ShowPanelTween(_constructionPanel.PanelTween);
        return true;
    }

    private void TryShowBuildingActionPanel(UnitSelectable selectable)
    {
        BuildingBase building = SelectableUtility.FindNear<BuildingBase>(selectable);

        if (building == null)
            return;

        _buildingActionPanel.Show(building);
        ShowPanelTween(_buildingActionPanel.PanelTween);
    }

    private void HideAll()
    {
        _workerCommandPanel.Hide();
        HidePanelTween(_workerCommandPanel.PanelTween);

        HideHousePanel();

        _productionBuildingPanel.Hide();
        HidePanelTween(_productionBuildingPanel.PanelTween);

        _buildingActionPanel.Hide();
        HidePanelTween(_buildingActionPanel.PanelTween);

        _armySelectionPanel.Hide();
        HidePanelTween(_armySelectionPanel.PanelTween);

        _constructionPanel.Hide();
        HidePanelTween(_constructionPanel.PanelTween);
    }

    public void HideHousePanel()
    {
        _housePanel.Hide();
        HidePanelTween(_housePanel.PanelTween);
    }

    private static void ShowPanelTween(SimplePanelTween panelTween)
    {
        panelTween.Show();
    }

    private static void HidePanelTween(SimplePanelTween panelTween)
    {
        panelTween.Hide();
    }
}
