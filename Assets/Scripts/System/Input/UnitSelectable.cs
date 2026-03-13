using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Компонент юнита, который ТОЛЬКО:
/// - показывает / скрывает визуал выделения
/// </summary>
public class UnitSelectable : MonoBehaviour
{
    [SerializeField] private GameObject selectionVisual;

    public bool IsSelected { get; private set; }

    private ISelectableEntity selectableEntity;

    private void Awake()
    {
        selectableEntity = GetComponent<ISelectableEntity>();

        if (selectionVisual != null)
            selectionVisual.SetActive(false);
    }

    public void Select(SelectionSystem selectionSystem)
    {
        IsSelected = true;

        if (selectionVisual != null)
            selectionVisual.SetActive(true);

        selectableEntity?.OnSelected(selectionSystem);
    }

    public void Deselect()
    {
        IsSelected = false;

        if (selectionVisual != null)
            selectionVisual.SetActive(false);

        selectableEntity?.OnDeselected();
    }
}
