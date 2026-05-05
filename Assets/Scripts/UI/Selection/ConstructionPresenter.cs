using UnityEngine;
using Zenject;

/// <summary>
/// ѕоказывает или скрывает панель строительства
/// </summary>
public class ConstructionPresenter : MonoBehaviour
{
    private SelectionSystem _selectionSystem;
    private ConstructionPanel _panel;
    private bool _isSubscribed;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        ConstructionPanel panel)
    {
        _selectionSystem = selectionSystem;
        _panel = panel;
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

    private void Subscribe()
    {
        if (_isSubscribed)
            return;

        if (_selectionSystem == null)
            return;

        _selectionSystem.SelectionChanged += OnSelectionChanged;
        _selectionSystem.SelectionCleared += OnSelectionCleared;

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
            return;

        if (_selectionSystem != null)
        {
            _selectionSystem.SelectionChanged -= OnSelectionChanged;
            _selectionSystem.SelectionCleared -= OnSelectionCleared;
        }

        _isSubscribed = false;
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