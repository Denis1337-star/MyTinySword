using UnityEngine;

/// <summary>
/// Слушает изменение выбранного объекта и,
/// если выбран worker, передаёт команду системе камеры сфокусироваться на нём.
/// </summary>
public class SelectionFocusPresenter : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private CameraFocusController focusController;

    /// <summary>
    /// Пытается восстановить отсутствующие ссылки через GameServices
    /// или через поиск по сцене.
    /// </summary>
    private void OnValidate()
    {
        if (selectionSystem == null)
        {
            if (GameServices.Instance != null && GameServices.Instance.Selection != null)
                selectionSystem = GameServices.Instance.Selection;
            else
                selectionSystem = FindObjectOfType<SelectionSystem>(true);
        }

        if (focusController == null)
        {
            if (GameServices.Instance != null && GameServices.Instance.CameraFocus != null)
                focusController = GameServices.Instance.CameraFocus;
            else
                focusController = FindObjectOfType<CameraFocusController>(true);
        }
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
    /// Если выбран worker, переводим фокус камеры на него
    /// </summary>
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