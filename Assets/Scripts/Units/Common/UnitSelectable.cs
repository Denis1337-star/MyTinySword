using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент юнита, который ТОЛЬКО:
/// - показывает / скрывает визуал выделения
/// </summary>
public class UnitSelectable : MonoBehaviour
{
    [SerializeField] private GameObject selectionVisual;  //подстветка что выделено

    public bool IsSelected { get; private set; }  //флаг выделен или нет

    private void Awake()
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(false);  //проверка
    }

    public void Select()
    {
        IsSelected = true;

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
