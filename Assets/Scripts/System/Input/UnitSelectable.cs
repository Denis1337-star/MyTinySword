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
        if (selectionVisual != null)
            selectionVisual.SetActive(false);
    }

    public void Select()
    {
        if (IsSelected)
            return;

        IsSelected = true;

        if (selectionVisual != null)
            selectionVisual.SetActive(true);
    }

    public void Deselect()
    {
        if (!IsSelected)
            return;

        IsSelected = false;

        if (selectionVisual != null)
            selectionVisual.SetActive(false);
    }
}
