using UnityEngine;
using Zenject;

/// <summary>
/// ѕоказывает или скрывает панель строительства
/// </summary>
public class ConstructionPresenter : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private ConstructionPanel _panel;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        ConstructionPanel panel)
    {
        _selectionSystem = selectionSystem;
        _panel = panel;
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

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        if (selectable == null)
        {
            _panel?.Hide();
            return;
        }

        ConstructionSlot slot = selectable.GetComponent<ConstructionSlot>();
        if (slot == null)
            slot = selectable.GetComponentInParent<ConstructionSlot>();

        if (slot == null)
        {
            _panel?.Hide();
            return;
        }

        if (!slot.HasConstruction)
        {
            _panel?.Show(slot);
            return;
        }

        _panel?.Hide();
    }

    private void OnSelectionCleared()
    {
        _panel?.Hide();
    }
}