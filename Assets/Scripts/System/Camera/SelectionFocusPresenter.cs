using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionFocusPresenter : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private CameraFocusController focusController;

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
        if (selectable == null || focusController == null)
            return;

        Worker worker = selectable.GetComponent<Worker>();
        if (worker == null)
            worker = selectable.GetComponentInParent<Worker>();

        if (worker != null)
            focusController.FocusOn(worker.transform);
    }

    private void OnSelectionCleared()
    {
        if (focusController != null)
            focusController.CancelFocus();
    }
}
