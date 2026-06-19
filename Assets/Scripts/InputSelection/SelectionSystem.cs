using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Центральная система выбора объектов игроком.
/// Выбирает только объекты игрока, чтобы enemy здания/юниты не открывали управляющие панели.
/// </summary>
public sealed class SelectionSystem : MonoBehaviour
{
    private const int MaxSelectionHits = 32;

    [SerializeField] private LayerMask _selectionLayerMask = ~0;

    private readonly Collider2D[] _selectionHits = new Collider2D[MaxSelectionHits];
    private readonly List<UnitSelectable> _selectedUnits = new();

    private Camera _mainCamera;

    public UnitSelectable CurrentSelection { get; private set; }
    public IReadOnlyList<UnitSelectable> SelectedUnits => _selectedUnits;

    public event Action<UnitSelectable> SelectionChanged;
    public event Action SelectionCleared;

    [Inject]
    private void Construct(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    private void OnDestroy()
    {
        SelectionChanged = null;
        SelectionCleared = null;
    }

    public bool TrySelectAtScreenPosition(Vector2 screenPosition)
    {
        if (!TryGetSelectableAtScreenPosition(screenPosition, out UnitSelectable selectable))
            return false;

        if (!TutorialInputGuard.AllowsSelectionOf(selectable))
            return false;

        Select(selectable);
        return true;
    }

    public bool TryGetSelectableAtScreenPosition(Vector2 screenPosition, out UnitSelectable selectable)
    {
        selectable = null;

        int hitCount = Physics2DHitUtility.OverlapAtScreen(
            _mainCamera,
            screenPosition,
            _selectionHits,
            _selectionLayerMask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _selectionHits[i];

            if (hit == null)
                continue;

            UnitSelectable foundSelectable = SelectableUtility.FindFromHit(hit);

            if (!CanSelectPlayerObject(foundSelectable))
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

        if (!CanSelectPlayerObject(selectable))
            return;

        if (!TutorialInputGuard.AllowsSelectionOf(selectable))
            return;

        if (CurrentSelection == selectable && _selectedUnits.Count <= 1)
        {
            selectable.Select();

            // Повторный tap по уже выбранному объекту должен обновить UI.
            SelectionChanged?.Invoke(selectable);
            return;
        }

        ClearSelectionInternal();

        CurrentSelection = selectable;
        CurrentSelection.Select();

        _selectedUnits.Add(selectable);

        SelectionChanged?.Invoke(selectable);
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

            if (!CanSelectPlayerObject(selectable))
                continue;

            selectable.Select();
            _selectedUnits.Add(selectable);
        }

        NotifySelectionResult();
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

            UnitSelectable selectable = SelectableUtility.FindFromComponent(armyUnit);

            if (!CanSelectPlayerObject(selectable))
                continue;

            selectable.Select();
            _selectedUnits.Add(selectable);
        }

        NotifySelectionResult();
    }

    public void SelectWorkerFromUI(Worker worker)
    {
        if (worker == null)
        {
            ClearSelection();
            return;
        }

        UnitSelectable selectable = SelectableUtility.FindFromComponent(worker);

        Select(selectable);
    }

    public void ClearSelection()
    {
        if (!TutorialInputGuard.AllowsClearSelection())
            return;

        ForceClearSelection();
    }

    /// <summary>
    /// Снимает выбор без проверки tutorial-guard (системные действия).
    /// </summary>
    public void ForceClearSelection()
    {
        if (CurrentSelection == null && _selectedUnits.Count == 0)
            return;

        ClearSelectionInternal();
        SelectionCleared?.Invoke();
    }

    private void NotifySelectionResult()
    {
        CurrentSelection = _selectedUnits.Count > 0 ? _selectedUnits[0] : null;

        if (CurrentSelection != null)
        {
            SelectionChanged?.Invoke(CurrentSelection);
            return;
        }

        SelectionCleared?.Invoke();
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

    private bool CanSelectPlayerObject(UnitSelectable selectable)
    {
        if (selectable == null)
            return false;

        if (!selectable.CanBeSelected)
            return false;

        FactionMember factionMember = selectable.GetComponent<FactionMember>();

        if (factionMember == null)
            return true;

        return factionMember.IsPlayer();
    }
}