using UnityEngine;

/// <summary>
/// Компонент юнита, который ТОЛЬКО:
/// - показывает / скрывает визуал выделения
/// </summary>
public class UnitSelectable : MonoBehaviour
{
    [SerializeField] private GameObject selectionVisual;

    public bool IsSelected { get; private set; }

    private void Awake()
    {
        ApplySelectionVisual(false);
    }

    private void OnValidate()
    {
        if (selectionVisual == null)
        {
            Transform child = transform.Find("Selection");
            if (child != null)
                selectionVisual = child.gameObject;
        }
    }

    public void Select()
    {
        if (IsSelected)
            return;

        IsSelected = true;
        ApplySelectionVisual(true);
    }

    public void Deselect()
    {
        if (!IsSelected)
            return;

        IsSelected = false;
        ApplySelectionVisual(false);
    }

    private void ApplySelectionVisual(bool value)
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(value);
    }
}
