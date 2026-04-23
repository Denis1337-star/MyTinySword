using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// —лушает изменение выбранного объекта и показывает подход€щую панель:
/// дл€ worker'а Ч WorkerCommandPanel,
/// дл€ дома Ч HousePanel,
/// дл€ производственного здани€ Ч ProductionBuildingPanel,
/// дл€ группы боевых юнитов Ч ArmySelectionPanel.
/// </summary>
public class SelectionUiPresenter : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;
    [SerializeField] private ProductionBuildingPanel productionBuildingPanel;
    [SerializeField] private ArmySelectionPanel armySelectionPanel;

    private void OnValidate()
    {
        if (selectionSystem == null)
            selectionSystem = FindObjectOfType<SelectionSystem>(true);

        if (workerCommandPanel == null)
            workerCommandPanel = FindObjectOfType<WorkerCommandPanel>(true);

        if (housePanel == null)
            housePanel = FindObjectOfType<HousePanel>(true);

        if (productionBuildingPanel == null)
            productionBuildingPanel = FindObjectOfType<ProductionBuildingPanel>(true);

        if (armySelectionPanel == null)
            armySelectionPanel = FindObjectOfType<ArmySelectionPanel>(true);
    }

    private void OnEnable()
    {
        if (selectionSystem == null)
            return;

        selectionSystem.SelectionChanged += OnSelectionChanged;
        selectionSystem.SelectionCleared += OnSelectionCleared;
    }

    private void OnDisable()
    {
        if (selectionSystem == null)
            return;

        selectionSystem.SelectionChanged -= OnSelectionChanged;
        selectionSystem.SelectionCleared -= OnSelectionCleared;
    }

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        HideAll();

        if (selectionSystem == null)
            return;

        IReadOnlyList<UnitSelectable> selectedUnits = selectionSystem.GetSelectedUnits();

        if (ContainsPlayerArmyUnits(selectedUnits))
        {
            if (armySelectionPanel != null)
                armySelectionPanel.Show(selectedUnits);

            return;
        }

        if (selectable == null)
            return;

        Worker worker = selectable.GetComponent<Worker>();
        if (worker == null)
            worker = selectable.GetComponentInParent<Worker>();

        if (worker != null)
        {
            if (workerCommandPanel != null)
                workerCommandPanel.ShowForWorker(worker);

            return;
        }

        House house = selectable.GetComponent<House>();
        if (house == null)
            house = selectable.GetComponentInParent<House>();

        if (house != null)
        {
            if (housePanel != null)
                housePanel.Show(house);

            return;
        }

        ProductionBuildingBase productionBuilding = selectable.GetComponent<ProductionBuildingBase>();
        if (productionBuilding == null)
            productionBuilding = selectable.GetComponentInParent<ProductionBuildingBase>();

        if (productionBuilding != null)
        {
            if (productionBuildingPanel != null)
                productionBuildingPanel.Show(productionBuilding);

            return;
        }
    }

    private void OnSelectionCleared()
    {
        HideAll();
    }

    private void HideAll()
    {
        if (workerCommandPanel != null)
            workerCommandPanel.Hide();

        if (housePanel != null)
            housePanel.Hide();

        if (productionBuildingPanel != null)
            productionBuildingPanel.Hide();

        if (armySelectionPanel != null)
            armySelectionPanel.Hide();
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