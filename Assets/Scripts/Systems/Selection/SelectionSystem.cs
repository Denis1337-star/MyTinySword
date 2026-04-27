using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;
using Zenject;

/// <summary>
/// Система выбора объектов игроком
/// </summary>
public class SelectionSystem : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private LayerMask _ignoreRaycastLayer;

    private readonly List<UnitSelectable> _selectedUnits = new();

    private Camera _mainCamera;
    private UnitSelectable _currentSelection;

    public event Action<UnitSelectable> SelectionChanged;
    public event Action SelectionCleared;

    [Inject]
    private void Construct(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (!TouchUtility.TryGetEndedTap(out var touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))
            return;

        ProcessTap(touch.screenPosition);
    }

    private void ProcessTap(Vector2 screenPosition)
    {
        if (_mainCamera == null)
            return;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        int mask = ~_ignoreRaycastLayer.value;
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, 100f, mask);

        if (hit.collider == null)
        {
            ClearSelection();
            return;
        }

        UnitSelectable selectable = hit.collider.GetComponentInParent<UnitSelectable>();
        if (selectable == null)
        {
            ClearSelection();
            return;
        }

        Select(selectable);
    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
            return;

        UnitSelectable selectable = worker.GetComponentInParent<UnitSelectable>();
        if (selectable == null)
            return;

        ClearSelectionInternal(notify: false);
        AddSingleSelection(selectable);

        SelectionChanged?.Invoke(_currentSelection);
    }

    public void Select(UnitSelectable selectable)
    {
        if (selectable == null)
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

    public void SelectArmyUnits(IReadOnlyList<ArmyUnit> units)
    {
        if (units == null || units.Count == 0)
            return;

        ClearSelectionInternal(notify: false);

        foreach (ArmyUnit unit in units)
        {
            if (unit == null)
                continue;

            UnitSelectable selectable = unit.GetComponentInParent<UnitSelectable>();
            if (selectable == null)
                continue;

            AddArmySelection(selectable);
        }

        SelectionChanged?.Invoke(_currentSelection);
    }

    public void ClearSelection()
    {
        if (_currentSelection == null && _selectedUnits.Count == 0)
            return;

        ClearSelectionInternal(notify: true);
    }

    public IReadOnlyList<UnitSelectable> GetSelectedUnits()
    {
        return _selectedUnits;
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
        _selectedUnits.Add(selectable);
        selectable.Select();

        _currentSelection = selectable;
    }

    private bool ContainsNonArmySelection()
    {
        foreach (UnitSelectable unit in _selectedUnits)
        {
            if (unit == null)
                continue;

            if (!IsPlayerArmyUnit(unit))
                return true;
        }

        return false;
    }

    private bool IsPlayerArmyUnit(UnitSelectable selectable)
    {
        if (selectable == null)
            return false;

        ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();
        if (armyUnit == null)
            armyUnit = selectable.GetComponentInParent<ArmyUnit>();

        return armyUnit != null && armyUnit.IsPlayerUnit();
    }

    private void ClearSelectionInternal(bool notify)
    {
        foreach (UnitSelectable unit in _selectedUnits)
        {
            if (unit != null)
                unit.Deselect();
        }

        _selectedUnits.Clear();
        _currentSelection = null;

        if (notify)
            SelectionCleared?.Invoke();
    }
}