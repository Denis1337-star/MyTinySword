using UnityEngine;

// <summary>
/// Показывает или скрывает панель строительства
/// в зависимости от выбранного объекта.
/// </summary>
public class ConstructionPresenter : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private ConstructionPanel panel;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
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

    private void ResolveReferences()
    {
        if (selectionSystem == null)
            selectionSystem = FindObjectOfType<SelectionSystem>(true);

        if (panel == null)
            panel = FindObjectOfType<ConstructionPanel>(true);
    }

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        Debug.Log("ConstructionPresenter: SelectionChanged", this);

        if (selectable == null)
        {
            Debug.Log("ConstructionPresenter: selectable == null", this);
            panel?.Hide();
            return;
        }

        ConstructionSlot slot = selectable.GetComponent<ConstructionSlot>();
        if (slot == null)
            slot = selectable.GetComponentInParent<ConstructionSlot>();

        if (slot == null)
        {
            Debug.Log("ConstructionPresenter: ConstructionSlot not found", selectable);
            panel?.Hide();
            return;
        }

        Debug.Log($"ConstructionPresenter: slot found = {slot.name}", slot);

        if (!slot.HasConstruction)
        {
            panel?.Show(slot);
            return;
        }

        panel?.Hide();
    }

    private void OnSelectionCleared()
    {
        Debug.Log("ConstructionPresenter: SelectionCleared", this);
        panel?.Hide();
    }
}