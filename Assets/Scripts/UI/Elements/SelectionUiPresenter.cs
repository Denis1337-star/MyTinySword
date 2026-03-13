using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUiPresenter : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private WorkerCommandPanel workerCommandPanel;
    [SerializeField] private HousePanel housePanel;

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
    }
}
