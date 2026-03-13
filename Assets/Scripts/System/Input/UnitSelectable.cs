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

    private void Awake()
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(false);

        Debug.Log($"[UnitSelectable] Awake on {name}", this);
    }

    public void Select()
    {
        IsSelected = true;

        Debug.Log($"[UnitSelectable] Select {name}", this);

        if (selectionVisual != null)
            selectionVisual.SetActive(true);
    }

    public void Deselect()
    {
        IsSelected = false;

        if (selectionVisual != null)
            selectionVisual.SetActive(false);
    }
}
