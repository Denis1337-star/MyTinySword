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
    [SerializeField] private GameObject selectionVisual;  //подстветка что выделено
    private CameraFocusController focusController;

    public bool IsSelected { get; private set; }  //флаг выделен или нет

    private void Awake()
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(false);  //проверка

                                    
        focusController = FindObjectOfType<CameraFocusController>();
    }

    public void Select()
    {
        IsSelected = true;

        // Фокус на основном объекте, а не на selectionVisual
        if (focusController != null)
            focusController.FocusOn(transform);

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
