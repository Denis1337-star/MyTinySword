using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Система команд для выбранной армии
/// </summary>
public sealed class CommandSystem : MonoBehaviour
{
    private const int MaxTargetHits = 32;

    [SerializeField] private LayerMask _targetLayerMask = ~0;

    private readonly Collider2D[] _targetHits = new Collider2D[MaxTargetHits];

    private SelectionSystem _selectionSystem;
    private Camera _mainCamera;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        Camera mainCamera)
    {
        _selectionSystem = selectionSystem;
        _mainCamera = mainCamera;
    }

    /// <summary>
    /// Пытается отдать выбранной армии команду атаки по enemy 
    /// </summary>
    public bool TryAttackSelectedArmyAtScreenPosition(Vector2 screenPosition)
    {
        if (_selectionSystem == null || _mainCamera == null)
            return false;

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

        return IssueAttackCommand(selectedUnits, target);
    }

    /// <summary>
    /// Пытается отдать выбранной армии команду движения в screenPosition
    /// </summary>
    public bool TryMoveSelectedArmyAtScreenPosition(Vector2 screenPosition)
    {
        if (_selectionSystem == null || _mainCamera == null)
            return false;

        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

        if (!HasPlayerArmyUnits(selectedUnits))
            return false;

        Vector2 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, screenPosition);

        return IssueMoveCommand(selectedUnits, worldPosition);
    }

    /// <summary>
    /// Старый общий метод оставляем для совместимости.
    /// Сначала пытается атаковать врага, если врага нет — двигает армию.
    /// </summary>
    public bool TryCommandSelectedArmyAtScreenPosition(Vector2 screenPosition)
    {
        if (TryAttackSelectedArmyAtScreenPosition(screenPosition))
            return true;

        return TryMoveSelectedArmyAtScreenPosition(screenPosition);
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

        damageable = hit.GetComponentInParent<IDamageable>();

        if (damageable != null)
            return damageable;

        return hit.GetComponentInChildren<IDamageable>();
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
        MonoBehaviour targetBehaviour = damageable as MonoBehaviour;

        if (targetBehaviour == null)
            return null;

        FactionMember factionMember = targetBehaviour.GetComponent<FactionMember>();

        if (factionMember != null)
            return factionMember;

        factionMember = targetBehaviour.GetComponentInParent<FactionMember>();

        if (factionMember != null)
            return factionMember;

        return targetBehaviour.GetComponentInChildren<FactionMember>();
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

        if (brain == null)
            brain = armyUnit.GetComponent<ArmyUnitBrain>();

        if (brain == null)
            brain = armyUnit.GetComponentInParent<ArmyUnitBrain>();

        if (brain == null)
            brain = armyUnit.GetComponentInChildren<ArmyUnitBrain>();

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
}