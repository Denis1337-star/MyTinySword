using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// Центральная система выбора объектов игроком
/// </summary>
public sealed class SelectionSystem : MonoBehaviour
{
    private const int MaxSelectionHits = 32;

    [SerializeField] private LayerMask _selectionLayerMask = ~0;

    private readonly Collider2D[] _selectionHits = new Collider2D[MaxSelectionHits];
    private readonly List<UnitSelectable> _selectedUnits = new();

    private readonly Subject<UnitSelectable> _selectionChanged = new();
    private readonly Subject<Unit> _selectionCleared = new();

    private Camera _mainCamera;

    public IObservable<UnitSelectable> SelectionChanged => _selectionChanged;
    public IObservable<Unit> SelectionCleared => _selectionCleared;

    public UnitSelectable CurrentSelection { get; private set; }
    public IReadOnlyList<UnitSelectable> SelectedUnits => _selectedUnits;

    [Inject]
    private void Construct(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    private void OnDestroy()
    {
        _selectionChanged.Dispose();
        _selectionCleared.Dispose();
    }

    public bool TrySelectAtScreenPosition(Vector2 screenPosition)
    {
        if (!TryGetSelectableAtScreenPosition(screenPosition, out UnitSelectable selectable))
            return false;

        Select(selectable);
        return true;
    }

    public bool TryGetSelectableAtScreenPosition(Vector2 screenPosition, out UnitSelectable selectable)
    {
        selectable = null;

        if (_mainCamera == null)
            return false;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        int hitCount = Physics2D.OverlapPointNonAlloc(
            worldPosition,
            _selectionHits,
            _selectionLayerMask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _selectionHits[i];
            if (hit == null)
                continue;

            UnitSelectable foundSelectable = FindSelectableFromHit(hit);

            if (foundSelectable == null)
                continue;

            if (!foundSelectable.CanBeSelected)
                continue;

            selectable = foundSelectable;
            return true;
        }

        return false;
    }

    public void Select(UnitSelectable selectable)
    {
        if (selectable == null)
        {
            ClearSelection();
            return;
        }

        if (!selectable.CanBeSelected)
            return;

        if (CurrentSelection == selectable && _selectedUnits.Count <= 1)
        {
            selectable.Select();

            // Повторный tap по уже выбранному объекту должен обновить UI
            _selectionChanged.OnNext(selectable);
            return;
        }

        ClearSelectionInternal();

        CurrentSelection = selectable;
        CurrentSelection.Select();

        _selectedUnits.Add(selectable);

        _selectionChanged.OnNext(selectable);
    }

    public void SelectArmyUnits(IReadOnlyList<UnitSelectable> armyUnits)
    {
        if (armyUnits == null || armyUnits.Count == 0)
        {
            ClearSelection();
            return;
        }

        ClearSelectionInternal();

        for (int i = 0; i < armyUnits.Count; i++)
        {
            UnitSelectable selectable = armyUnits[i];

            if (selectable == null || !selectable.CanBeSelected)
                continue;

            selectable.Select();
            _selectedUnits.Add(selectable);
        }

        CurrentSelection = _selectedUnits.Count > 0 ? _selectedUnits[0] : null;

        if (CurrentSelection != null)
            _selectionChanged.OnNext(CurrentSelection);
        else
            _selectionCleared.OnNext(Unit.Default);
    }

    public void SelectArmyUnits(IReadOnlyList<ArmyUnit> armyUnits)
    {
        if (armyUnits == null || armyUnits.Count == 0)
        {
            ClearSelection();
            return;
        }

        ClearSelectionInternal();

        for (int i = 0; i < armyUnits.Count; i++)
        {
            ArmyUnit armyUnit = armyUnits[i];
            if (armyUnit == null)
                continue;

            UnitSelectable selectable = FindSelectableFromComponent(armyUnit);

            if (selectable == null || !selectable.CanBeSelected)
                continue;

            selectable.Select();
            _selectedUnits.Add(selectable);
        }

        CurrentSelection = _selectedUnits.Count > 0 ? _selectedUnits[0] : null;

        if (CurrentSelection != null)
            _selectionChanged.OnNext(CurrentSelection);
        else
            _selectionCleared.OnNext(Unit.Default);
    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
        {
            ClearSelection();
            return;
        }

        UnitSelectable selectable = FindSelectableFromComponent(worker);

        Select(selectable);
    }

    public void ClearSelection()
    {
        ClearSelectionInternal();
        _selectionCleared.OnNext(Unit.Default);
    }

    private void ClearSelectionInternal()
    {
        for (int i = 0; i < _selectedUnits.Count; i++)
        {
            UnitSelectable selectedUnit = _selectedUnits[i];

            if (selectedUnit == null)
                continue;

            selectedUnit.Deselect();
        }

        _selectedUnits.Clear();
        CurrentSelection = null;
    }

    private UnitSelectable FindSelectableFromHit(Collider2D hit)
    {
        if (hit == null)
            return null;

        UnitSelectable selectable = hit.GetComponentInParent<UnitSelectable>();

        if (selectable != null)
            return selectable;

        return hit.GetComponentInChildren<UnitSelectable>();
    }

    private UnitSelectable FindSelectableFromComponent(Component component)
    {
        if (component == null)
            return null;

        UnitSelectable selectable = component.GetComponent<UnitSelectable>();

        if (selectable != null)
            return selectable;

        selectable = component.GetComponentInParent<UnitSelectable>();

        if (selectable != null)
            return selectable;

        return component.GetComponentInChildren<UnitSelectable>();
    }
}