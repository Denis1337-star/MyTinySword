using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет отображением UI панелей в зависимости от текущего выбора
/// </summary>
public sealed class SelectionUiPresenter : MonoBehaviour
{
    [SerializeField] private WorkerCommandPanel _workerCommandPanel;
    [SerializeField] private HousePanel _housePanel;
    [SerializeField] private ProductionBuildingPanel _productionBuildingPanel;
    [SerializeField] private ArmySelectionPanel _armySelectionPanel;

    private SelectionSystem _selectionSystem;
    private CompositeDisposable _disposables;

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

    private void Awake()
    {
        HideAll();
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
        DisposeSubscriptions();
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
    }

    private void HandleSelectionCleared(Unit _)
    {
        HideAll();
    }

    private void Subscribe()
    {
        if (_selectionSystem == null)
            return;

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
        if (_selectionSystem == null)
        {
            HideAll();
            return;
        }

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
        if (_selectionSystem == null || _armySelectionPanel == null)
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
        if (_workerCommandPanel == null)
            return false;

        Worker worker = FindComponentNearSelectable<Worker>(selectable);

        if (worker == null)
            return false;

        _workerCommandPanel.ShowForWorker(worker);
        return true;
    }

    private bool TryShowHousePanel(UnitSelectable selectable)
    {
        if (_housePanel == null)
            return false;

        House house = FindComponentNearSelectable<House>(selectable);

        if (house == null)
            return false;

        _housePanel.Show(house);
        return true;
    }

    private bool TryShowProductionBuildingPanel(UnitSelectable selectable)
    {
        if (_productionBuildingPanel == null)
            return false;

        ProductionBuildingBase building = FindComponentNearSelectable<ProductionBuildingBase>(selectable);

        if (building == null)
            return false;

        _productionBuildingPanel.Show(building);
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
        _workerCommandPanel?.Hide();
        _housePanel?.Hide();
        _productionBuildingPanel?.Hide();
        _armySelectionPanel?.Hide();
    }
}