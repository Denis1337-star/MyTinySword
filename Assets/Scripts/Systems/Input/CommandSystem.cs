using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Система пользовательских команд
/// </summary>
public class CommandSystem : MonoBehaviour
{
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

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!TouchUtility.TryGetEndedTap(out var touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))
            return;

        if (_selectionSystem == null || _mainCamera == null)
            return;

        IReadOnlyList<UnitSelectable> selected = _selectionSystem.GetSelectedUnits();
        if (selected == null || selected.Count == 0)
            return;

        Vector3 worldPosition = TouchUtility.ScreenToWorld(_mainCamera, touch.screenPosition);

        if (!ContainsPlayerArmyUnits(selected))
            return;

        IDamageable target = FindEnemyDamageableAt(worldPosition, selected);

        foreach (UnitSelectable selectable in selected)
        {
            if (selectable == null)
                continue;

            ArmyUnitBrain brain = selectable.GetComponent<ArmyUnitBrain>();
            if (brain == null)
                brain = selectable.GetComponentInParent<ArmyUnitBrain>();

            if (brain == null)
                continue;

            if (target != null)
                brain.Attack(target);
            else
                brain.MoveTo(worldPosition);
        }
    }

    private bool ContainsPlayerArmyUnits(IReadOnlyList<UnitSelectable> selected)
    {
        foreach (UnitSelectable selectable in selected)
        {
            if (selectable == null)
                continue;

            ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();
            if (armyUnit == null)
                armyUnit = selectable.GetComponentInParent<ArmyUnit>();

            if (armyUnit != null && armyUnit.IsPlayerUnit())
                return true;
        }

        return false;
    }

    private IDamageable FindEnemyDamageableAt(
        Vector3 worldPosition,
        IReadOnlyList<UnitSelectable> selected)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        if (hit == null)
            return null;

        IDamageable damageable = hit.GetComponent<IDamageable>();
        if (damageable == null)
            damageable = hit.GetComponentInParent<IDamageable>();

        if (damageable == null || damageable.IsDead)
            return null;

        MonoBehaviour targetBehaviour = damageable as MonoBehaviour;
        if (targetBehaviour == null)
            return null;

        FactionMember targetFaction = targetBehaviour.GetComponent<FactionMember>();
        if (targetFaction == null)
            targetFaction = targetBehaviour.GetComponentInParent<FactionMember>();

        if (targetFaction == null)
            return null;

        foreach (UnitSelectable selectable in selected)
        {
            if (selectable == null)
                continue;

            ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();
            if (armyUnit == null)
                armyUnit = selectable.GetComponentInParent<ArmyUnit>();

            if (armyUnit == null || !armyUnit.IsPlayerUnit())
                continue;

            FactionMember unitFaction = armyUnit.FactionMember;
            if (unitFaction == null)
                continue;

            return unitFaction.IsEnemy(targetFaction) ? damageable : null;
        }

        return null;
    }
}