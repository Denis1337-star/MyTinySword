using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Система команд для выбранной армии.
/// </summary>
public sealed class CommandSystem : MonoBehaviour
{
    private const int MaxTargetHits = 32;

    [SerializeField] private LayerMask _targetLayerMask = ~0;
    [SerializeField] private MoveCommandIndicator _moveCommandIndicator;

    private readonly Collider2D[] _targetHits = new Collider2D[MaxTargetHits];

    private SelectionSystem _selectionSystem;
    private Camera _mainCamera;

    public event Action<IDamageable> AttackCommandIssued;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        Camera mainCamera)
    {
        _selectionSystem = selectionSystem;
        _mainCamera = mainCamera;
    }

    /// <summary>
    /// Пытается отдать выбранной армии команду атаки по врагу.
    /// </summary>
    public bool TryAttackSelectedArmyAtScreenPosition(Vector2 screenPosition)
    {
        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

        if (!HasPlayerArmyUnits(selectedUnits))
            return false;

        FactionType? selectedFaction = FindFirstSelectedPlayerFactionType(selectedUnits);

        if (selectedFaction == null)
            return false;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        IDamageable target = FindEnemyDamageableAt(worldPosition, selectedFaction.Value);

        if (target == null)
            return false;

        bool commandIssued = IssueAttackCommand(selectedUnits, target);

        if (commandIssued)
            AttackCommandIssued?.Invoke(target);

        return commandIssued;
    }

    /// <summary>
    /// Пытается отдать выбранной армии команду движения в screenPosition.
    /// </summary>
    public bool TryMoveSelectedArmyAtScreenPosition(Vector2 screenPosition)
    {
        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

        if (!HasPlayerArmyUnits(selectedUnits))
            return false;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        bool commandIssued = IssueMoveCommand(selectedUnits, worldPosition);

        if (commandIssued)
            ShowMoveCommandIndicator(worldPosition);

        return commandIssued;
    }

    private bool IssueAttackCommand(
        IReadOnlyList<UnitSelectable> selectedUnits,
        IDamageable target)
    {
        if (selectedUnits == null || target == null)
            return false;

        bool commandIssued = false;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (!TryGetPlayerArmyBrain(selectedUnits[i], out ArmyUnitBrain brain))
                continue;

            brain.Attack(target);
            commandIssued = true;
        }

        return commandIssued;
    }

    private bool IssueMoveCommand(
        IReadOnlyList<UnitSelectable> selectedUnits,
        Vector2 worldPosition)
    {
        if (selectedUnits == null)
            return false;

        bool commandIssued = false;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (!TryGetPlayerArmyBrain(selectedUnits[i], out ArmyUnitBrain brain))
                continue;

            brain.MoveTo(worldPosition);
            commandIssued = true;
        }

        return commandIssued;
    }

    private void ShowMoveCommandIndicator(Vector2 worldPosition)
    {
        if (_moveCommandIndicator == null)
            return;

        _moveCommandIndicator.Show(worldPosition);
    }

    private IDamageable FindEnemyDamageableAt(Vector2 worldPosition, FactionType selectedArmyFaction)
    {

        int hitCount = Physics2DHitUtility.OverlapAtWorld(
            worldPosition,
            _targetHits,
            _targetLayerMask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _targetHits[i];

            if (hit == null)
                continue;

            IDamageable damageable = FindDamageable(hit);

            if (damageable == null)
                continue;

            if (!IsValidEnemyTarget(damageable, selectedArmyFaction))
                continue;

            return damageable;
        }

        return null;
    }

    private IDamageable FindDamageable(Collider2D hit)
    {
        if (hit == null)
            return null;

        return hit.GetComponent<IDamageable>();
    }

    private bool IsValidEnemyTarget(IDamageable damageable, FactionType selectedArmyFaction)
    {
        if (damageable == null || damageable.IsDead)
            return false;

        FactionType? targetFaction = FactionResolver.TryGetFaction(damageable as Component);

        if (targetFaction == null)
            return false;

        return FactionRules.IsEnemy(selectedArmyFaction, targetFaction.Value);
    }

    private bool HasPlayerArmyUnits(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        return ArmyUnitSelectionUtility.HasAnyPlayerArmyUnit(selectedUnits);
    }

    private FactionType? FindFirstSelectedPlayerFactionType(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        if (selectedUnits == null)
            return null;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (!ArmyUnitSelectionUtility.TryGetPlayerArmyUnit(selectedUnits[i], out ArmyUnit armyUnit))
                continue;

            return armyUnit.Faction;
        }

        return null;
    }

    private bool TryGetPlayerArmyBrain(
        UnitSelectable selectable,
        out ArmyUnitBrain brain)
    {
        brain = null;

        if (!ArmyUnitSelectionUtility.TryGetPlayerArmyUnit(selectable, out ArmyUnit armyUnit))
            return false;

        brain = armyUnit.Brain;
        return brain != null;
    }

    private void OnDestroy()
    {
        AttackCommandIssued = null;
    }
}
