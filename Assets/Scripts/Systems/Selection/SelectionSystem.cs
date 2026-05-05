using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Система выбора игровых объектов
/// </summary>
public sealed class SelectionSystem : MonoBehaviour
{
    private const int MaxSelectionHits = 32;
    [SerializeField] private LayerMask _ignoreRaycastLayer;
    private readonly List<UnitSelectable> _selectedUnits = new();
    private readonly Collider2D[] _selectionHits = new Collider2D[MaxSelectionHits];

    private Camera _mainCamera;
    private UnitSelectable _currentSelection;

    public event Action<UnitSelectable> SelectionChanged;
    public event Action SelectionCleared;

    public UnitSelectable CurrentSelection => _currentSelection;
    public IReadOnlyList<UnitSelectable> SelectedUnits => _selectedUnits;

    public bool HasSelection => _currentSelection != null || _selectedUnits.Count > 0;
    public bool HasPlayerArmySelection => ContainsPlayerArmyUnits(_selectedUnits);

    [Inject]
    private void Construct(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    /// <summary>
    /// Ищет selectable объект под экранной позицией
    /// </summary>
    public bool TryGetSelectableAtScreenPosition(
        Vector2 screenPosition,
        out UnitSelectable selectable)
    {
        selectable = null;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        int layerMask = ~_ignoreRaycastLayer.value;

        int hitCount = Physics2D.OverlapPointNonAlloc(
            worldPosition,
            _selectionHits,
            layerMask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _selectionHits[i];
            if (hit == null)
                continue;

            UnitSelectable foundSelectable = hit.GetComponentInParent<UnitSelectable>();
            if (foundSelectable == null)
                continue;

            if (!foundSelectable.CanBeSelected)
                continue;

            selectable = foundSelectable;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Выбирает worker из UI-списка
    /// </summary>
    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponentInParent<UnitSelectable>();
        if (selectable == null)
            return;

        if (!selectable.CanBeSelected)
            return;

        ClearSelectionInternal(notify: false);
        AddSingleSelection(selectable);

        SelectionChanged?.Invoke(_currentSelection);
    }

    /// <summary>
    /// Выбирает  selectable объект
    /// </summary>
    public void Select(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        if (!selectable.CanBeSelected)
            return;

        if (IsPlayerArmyUnit(selectable))
        {
            AddArmySelection(selectable);
            SelectionChanged?.Invoke(_currentSelection);
            return;
        }

        ClearSelectionInternal(notify: false);
        AddSingleSelection(selectable);

        SelectionChanged?.Invoke(_currentSelection);
    }

    /// <summary>
    /// Выбирает список army units
    /// </summary>
    public void SelectArmyUnits(IReadOnlyList<ArmyUnit> units)
    {
        if (units == null || units.Count == 0)
            return;

        ClearSelectionInternal(notify: false);

        for (int i = 0; i < units.Count; i++)
        {
            ArmyUnit unit = units[i];
            if (unit == null)
                continue;

            if (!unit.IsPlayerUnit())
                continue;

            UnitSelectable selectable = unit.GetComponentInParent<UnitSelectable>();
            if (selectable == null)
                continue;

            if (!selectable.CanBeSelected)
                continue;

            AddArmySelection(selectable);
        }

        SelectionChanged?.Invoke(_currentSelection);
    }

    /// <summary>
    /// Полностью очищает текущий выбор
    /// </summary>
    public void ClearSelection()
    {
        if (!HasSelection)
            return;

        ClearSelectionInternal(notify: true);
    }

    private void AddArmySelection(UnitSelectable selectable)
    {
        if (_selectedUnits.Contains(selectable))
        {
            _currentSelection = selectable;
            return;
        }

        if (ContainsNonArmySelection())
            ClearSelectionInternal(notify: false);

        _selectedUnits.Add(selectable);
        selectable.Select();

        _currentSelection = selectable;
    }

    private void AddSingleSelection(UnitSelectable selectable)
    {
        ClearSelectionInternal(notify: false);

        _selectedUnits.Add(selectable);
        selectable.Select();

        _currentSelection = selectable;
    }

    private bool ContainsNonArmySelection()
    {
        for (int i = 0; i < _selectedUnits.Count; i++)
        {
            UnitSelectable selectable = _selectedUnits[i];
            if (selectable == null)
                continue;

            if (!IsPlayerArmyUnit(selectable))
                return true;
        }

        return false;
    }

    private bool ContainsPlayerArmyUnits(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        if (selectedUnits == null)
            return false;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            UnitSelectable selectable = selectedUnits[i];
            if (selectable == null)
                continue;

            if (IsPlayerArmyUnit(selectable))
                return true;
        }

        return false;
    }

    private bool IsPlayerArmyUnit(UnitSelectable selectable)
    {
        if (selectable == null)
            return false;

        ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();

        return armyUnit != null && armyUnit.IsPlayerUnit();
    }

    private void ClearSelectionInternal(bool notify)
    {
        for (int i = 0; i < _selectedUnits.Count; i++)
        {
            UnitSelectable selectable = _selectedUnits[i];

            if (selectable != null)
                selectable.Deselect();
        }

        _selectedUnits.Clear();
        _currentSelection = null;

        if (notify)
            SelectionCleared?.Invoke();
    }
}