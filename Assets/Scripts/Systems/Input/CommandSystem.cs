using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Система пользовательских команд
/// Отвечает за обработку тапа по миру и отдачу команды движения
/// выбранным юнитам, которые поддерживают ручное перемещение
/// </summary>
public class CommandSystem : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    /// <summary>
    /// Пытается заполнить отсутствующие ссылки через GameServices
    /// или через прямой поиск по сцене
    /// </summary>
    private void ResolveReferences()
    {
        if (selectionSystem == null)
        {
            if (GameServices.Instance != null && GameServices.Instance.SelectionSystem != null)
                selectionSystem = GameServices.Instance.SelectionSystem;
            else
                selectionSystem = FindObjectOfType<SelectionSystem>(true);
        }

        if (mainCamera == null)
        {
            if (GameServices.Instance != null && GameServices.Instance.MainCamera != null)
                mainCamera = GameServices.Instance.MainCamera;
            else
                mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!TouchUtility.TryGetEndedTouch(out var touch))
            return;

        if (TouchUtility.IsPointerOverUI(touch))
            return;

        if (selectionSystem == null || mainCamera == null)
            return;

        IReadOnlyList<UnitSelectable> selected = selectionSystem.GetSelectedUnits();
        if (selected == null || selected.Count == 0)
            return;

        Vector3 worldPos = TouchUtility.ScreenToWorld(mainCamera, touch.screenPosition);

        // Работать должны только боевые юниты игрока.
        if (!ContainsPlayerArmyUnits(selected))
            return;

        IDamageable target = FindEnemyDamageableAt(worldPos, selected);

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
                brain.MoveTo(worldPos);
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

    private IDamageable FindEnemyDamageableAt(Vector3 worldPos, IReadOnlyList<UnitSelectable> selected)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
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

        // Достаточно, чтобы первый выбранный боевой юнит подтвердил,
        // что цель действительно вражеская.
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

            if (unitFaction.IsEnemy(targetFaction))
                return damageable;

            return null;
        }

        return null;
    }
}