using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет отображением UI-панелей в зависимости от текущего выбора
/// </summary>
public sealed class SelectionUiPresenter : ValidatedMonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private WorkerCommandPanel _workerCommandPanel;
    [SerializeField] private HousePanel _housePanel;
    [SerializeField] private ProductionBuildingPanel _productionBuildingPanel;
    [SerializeField] private ArmySelectionPanel _armySelectionPanel;
    [SerializeField] private ConstructionPanel _constructionPanel;

    private SelectionSystem _selectionSystem;
    private CompositeDisposable _disposables;

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

    private void OnDisable()
    {
        DisposeSubscriptions();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _workerCommandPanel, nameof(_workerCommandPanel));
        valid &= ValidationUtility.IsAssigned(this, _housePanel, nameof(_housePanel));
        valid &= ValidationUtility.IsAssigned(this, _productionBuildingPanel, nameof(_productionBuildingPanel));
        valid &= ValidationUtility.IsAssigned(this, _armySelectionPanel, nameof(_armySelectionPanel));
        valid &= ValidationUtility.IsAssigned(this, _constructionPanel, nameof(_constructionPanel));

        return valid;
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

        TryShowConstructionPanel(selectable);
    }

    private void HandleSelectionCleared(Unit _)
    {
        HideAll();
    }

    private void Subscribe()
    {

        if (_disposables != null)
            return;

        _disposables = new CompositeDisposable();

        _selectionSystem.SelectionChanged
            .Subscribe(HandleSelectionChanged)
            .AddTo(_disposables);

        _selectionSystem.SelectionCleared
            .Subscribe(HandleSelectionCleared)
            .AddTo(_disposables);

        RefreshFromCurrentSelection();
    }

    private void DisposeSubscriptions()
    {
        _disposables?.Dispose();
        _disposables = null;
    }

    private void RefreshFromCurrentSelection()
    {
        UnitSelectable currentSelection = _selectionSystem.CurrentSelection;

        if (currentSelection == null)
        {
            HandleSelectionCleared(Unit.Default);
            return;
        }

        HandleSelectionChanged(currentSelection);
    }

    private bool TryShowArmyPanel()
    {
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

    private T FindComponentNearSelectable<T>(UnitSelectable selectable) where T : Component
    {
        if (selectable == null)
            return null;

        T component = selectable.GetComponent<T>();

            return component;
    }

    private void HideAll()
    {
        _workerCommandPanel.Hide();
        _housePanel.Hide();
        _productionBuildingPanel.Hide();
        _armySelectionPanel.Hide();
        _constructionPanel.Hide();
    }
}