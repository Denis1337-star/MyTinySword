using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// —истема команд дл€ выбранной армии.
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
    /// ѕытаетс€ отдать выбранной армии команду атаки по enemy.
    /// </summary>
    public bool TryAttackSelectedArmyAtScreenPosition(Vector2 screenPosition)
    {
        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

        if (!HasPlayerArmyUnits(selectedUnits))
            return false;

        FactionMember selectedArmyFaction = FindFirstSelectedPlayerFaction(selectedUnits);

        if (selectedArmyFaction == null)
            return false;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        IDamageable target = FindEnemyDamageableAt(worldPosition, selectedArmyFaction);

        if (target == null)
            return false;

        bool commandIssued = IssueAttackCommand(selectedUnits, target);

        if (commandIssued)
            AttackCommandIssued?.Invoke(target);

        return commandIssued;
    }

    /// <summary>
    /// ѕытаетс€ отдать выбранной армии команду движени€ в screenPosition.
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

    private IDamageable FindEnemyDamageableAt(
        Vector2 worldPosition,
        FactionMember selectedArmyFaction)
    {
        if (selectedArmyFaction == null)
            return null;

        int hitCount = Physics2D.OverlapPointNonAlloc(
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

        IDamageable damageable = hit.GetComponent<IDamageable>();

        if (damageable != null)
            return damageable;

        Component parentComponent = hit.GetComponentInParent<Component>();

        return parentComponent != null
            ? parentComponent.GetComponentInParent<IDamageable>()
            : null;
    }

    private bool IsValidEnemyTarget(
        IDamageable damageable,
        FactionMember selectedArmyFaction)
    {
        if (damageable == null || damageable.IsDead)
            return false;

        if (selectedArmyFaction == null)
            return false;

        FactionMember targetFaction = FindFactionMember(damageable);

        if (targetFaction == null)
            return false;

        return selectedArmyFaction.IsEnemy(targetFaction);
    }

    private FactionMember FindFactionMember(IDamageable damageable)
    {
        Component targetComponent = damageable as Component;

        if (targetComponent == null)
            return null;

        FactionMember factionMember = targetComponent.GetComponent<FactionMember>();

        if (factionMember != null)
            return factionMember;

        return targetComponent.GetComponentInParent<FactionMember>();
    }

    private bool HasPlayerArmyUnits(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        if (selectedUnits == null || selectedUnits.Count == 0)
            return false;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (TryGetPlayerArmyUnit(selectedUnits[i], out ArmyUnit _))
                return true;
        }

        return false;
    }

    private FactionMember FindFirstSelectedPlayerFaction(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        if (selectedUnits == null)
            return null;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (!TryGetPlayerArmyUnit(selectedUnits[i], out ArmyUnit armyUnit))
                continue;

            if (armyUnit.FactionMember != null)
                return armyUnit.FactionMember;
        }

        return null;
    }

    private bool TryGetPlayerArmyBrain(
        UnitSelectable selectable,
        out ArmyUnitBrain brain)
    {
        brain = null;

        if (!TryGetPlayerArmyUnit(selectable, out ArmyUnit armyUnit))
            return false;

        brain = armyUnit.Brain;
        return brain != null;
    }

    private bool TryGetPlayerArmyUnit(
        UnitSelectable selectable,
        out ArmyUnit armyUnit)
    {
        armyUnit = null;

        if (selectable == null)
            return false;

        armyUnit = selectable.GetComponent<ArmyUnit>();

        if (armyUnit == null)
            armyUnit = selectable.GetComponentInParent<ArmyUnit>();

        if (armyUnit == null)
            armyUnit = selectable.GetComponentInChildren<ArmyUnit>();

        if (armyUnit == null)
            return false;

        if (!armyUnit.IsPlayerUnit())
            return false;

        if (armyUnit.IsDead)
            return false;

        return true;
    }

    private void OnDestroy()
    {
        AttackCommandIssued = null;
    }
}