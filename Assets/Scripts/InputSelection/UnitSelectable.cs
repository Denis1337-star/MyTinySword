using UnityEngine;

/// <summary>
/// Компонент визуального выделения объекта
/// </summary>
public sealed class UnitSelectable : ValidatedMonoBehaviour
{
    [SerializeField] private GameObject _selectionVisual;
    [SerializeField] private bool _canBeSelected = true;

    public bool IsSelected { get; private set; }
    public bool CanBeSelected => _canBeSelected;

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        ApplySelectionVisual(false);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _selectionVisual, nameof(_selectionVisual));

        return valid;
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

    public void SetCanBeSelected(bool canBeSelected)
    {
        _canBeSelected = canBeSelected;

        if (!_canBeSelected)
            Deselect();
    }

    private void ApplySelectionVisual(bool isVisible)
    {
        if (_selectionVisual == gameObject)
            return;

        _selectionVisual.SetActive(isVisible);
    }
}