using UnityEngine;

/// <summary>
/// —лушает изменение выбранного объекта и показывает подход€щую панель:
/// дл€ worker'а Ч WorkerCommandPanel,
/// дл€ дома Ч HousePanel,
/// дл€ производственного здани€ Ч ProductionBuildingPanel.
/// </summary>
public class SelectionUiPresenter : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;
    [SerializeField] private ProductionBuildingPanel productionBuildingPanel;

    /// <summary>
    /// ѕытаетс€ восстановить отсутствующие ссылки на selection system и UI-панели.
    /// </summary>
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

    /// <summary>
    /// ѕоказывает нужную панель в зависимости от выбранного объекта.
    /// </summary>
    private void OnSelectionChanged(UnitSelectable selectable)
    {
        HideAll();

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
    }
}