using UnityEngine;

/// <summary>
/// —лушает изменение выбранного объекта и показывает подход€щую панель:
/// дл€ worker'а Ч WorkerCommandPanel, дл€ дома Ч HousePanel.
/// </summary>
public class SelectionUiPresenter : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;

    /// <summary>
    /// ѕытаетс€ восстановить отсутствующие ссылки на selection system и UI-панели
    /// </summary>
    private void OnValidate()
    {
        if (selectionSystem == null)
            selectionSystem = FindObjectOfType<SelectionSystem>(true);

        if (workerCommandPanel == null)
            workerCommandPanel = FindObjectOfType<WorkerCommandPanel>(true);

        if (housePanel == null)
            housePanel = FindObjectOfType<HousePanel>(true);
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
    /// ѕытаетс€ восстановить отсутствующие ссылки на selection system и UI-панели
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

        if (house != null && housePanel != null)
            housePanel.Show(house);
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
    }
}
