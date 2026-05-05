using UnityEngine;

/// <summary>
/// Компонент визуального выделения объекта
/// </summary>
public sealed class UnitSelectable : MonoBehaviour
{
    [SerializeField] private GameObject _selectionVisual;
    [SerializeField] private bool _canBeSelected = true;

    public bool IsSelected { get; private set; }
    public bool CanBeSelected => _canBeSelected;

    private void Awake()
    {
        ApplySelectionVisual(false);
    }

    private void OnValidate()
    {
        if (_selectionVisual == null)
        {
            Transform selectionTransform = transform.Find("Selection");

            if (selectionTransform != null)
                _selectionVisual = selectionTransform.gameObject;
        }
    }

    public void Select()
    {
        if (!_canBeSelected)
            return;

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

    /// <summary>
    /// Позволяет включить или выключить возможность выбора во время игры
    /// </summary>
    public void SetCanBeSelected(bool canBeSelected)
    {
        _canBeSelected = canBeSelected;

        if (!_canBeSelected)
            Deselect();
    }

    private void ApplySelectionVisual(bool isVisible)
    {
        if (_selectionVisual == null)
            return;

        if (_selectionVisual == gameObject)
            return;

        _selectionVisual.SetActive(isVisible);
    }
}