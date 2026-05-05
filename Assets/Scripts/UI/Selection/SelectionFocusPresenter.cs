using UnityEngine;
using Zenject;

/// <summary>
/// Связывает выбор объекта с focus камеры
/// </summary>
public sealed class SelectionFocusPresenter : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private CameraFocusController _focusController;

    private bool _isSubscribed;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        CameraFocusController focusController)
    {
        _selectionSystem = selectionSystem;
        _focusController = focusController;
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
        Unsubscribe();
    }

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        if (selectable == null || _focusController == null)
            return;

        Worker worker = selectable.GetComponent<Worker>();
        if (worker == null)
            worker = selectable.GetComponentInParent<Worker>();

        if (worker == null)
            return;

        _focusController.FocusOn(worker.transform);
    }

    private void OnSelectionCleared()
    {
        _focusController?.CancelFocus();
    }

    private void Subscribe()
    {
        if (_isSubscribed)
            return;

        _selectionSystem.SelectionChanged += OnSelectionChanged;
        _selectionSystem.SelectionCleared += OnSelectionCleared;

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
            return;


        _selectionSystem.SelectionChanged -= OnSelectionChanged;
        _selectionSystem.SelectionCleared -= OnSelectionCleared;


        _isSubscribed = false;
    }
}