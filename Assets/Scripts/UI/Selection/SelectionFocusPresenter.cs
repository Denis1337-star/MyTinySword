using UnityEngine;
using Zenject;

/// <summary>
/// Слушает изменение выбранного объекта 
/// если выбран worker, передаёт команду системе камеры сфокусироваться на нём
/// </summary>
public class SelectionFocusPresenter : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private CameraFocusController _focusController;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        CameraFocusController focusController)
    {
        _selectionSystem = selectionSystem;
        _focusController = focusController;
    }

    private void OnEnable()
    {
        if (_selectionSystem == null)
            return;

        _selectionSystem.SelectionChanged += OnSelectionChanged;
        _selectionSystem.SelectionCleared += OnSelectionCleared;
    }

    private void OnDisable()
    {
        if (_selectionSystem == null)
            return;

        _selectionSystem.SelectionChanged -= OnSelectionChanged;
        _selectionSystem.SelectionCleared -= OnSelectionCleared;
    }

    /// <summary>
    /// Если выбран worker, переводит камеру в follow-режим 
    /// </summary>
    private void OnSelectionChanged(UnitSelectable selectable)
    {
        if (selectable == null || _focusController == null)
            return;

        Worker worker = selectable.GetComponent<Worker>();
        if (worker == null)
            worker = selectable.GetComponentInParent<Worker>();

        if (worker != null)
            _focusController.FocusOn(worker.transform);
    }

    /// <summary>
    /// При очистке выбора отменяет focus-режим камеры
    /// </summary>
    private void OnSelectionCleared()
    {
        _focusController?.CancelFocus();
    }
}