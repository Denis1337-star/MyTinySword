using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Глобальный реестр всех боевых юнитов 
/// </summary>
public sealed class ArmyUnitRegistry : MonoBehaviour
{
    [SerializeField, Min(1)] private int _maxPlayerArmyUnits = 10;

    private readonly List<ArmyUnit> _allUnits = new();

    private int _reservedPlayerArmySlots;
    private TechTreeBonusService _techTreeBonusService;

    public IReadOnlyList<ArmyUnit> AllUnits => _allUnits;

    public int MaxPlayerArmyUnits => _maxPlayerArmyUnits + GetArmyCapBonus();

    public int CurrentPlayerArmyUnits => CountPlayerUnits();

    public int ReservedPlayerArmySlots => _reservedPlayerArmySlots;

    public int CommittedPlayerArmySlots => CurrentPlayerArmyUnits + _reservedPlayerArmySlots;

    public int FreePlayerArmySlots => Mathf.Max(
        0,
        _maxPlayerArmyUnits - CommittedPlayerArmySlots);

    public event Action OnArmyChanged;

    private void OnValidate()
    {
        _maxPlayerArmyUnits = Mathf.Max(1, _maxPlayerArmyUnits);
    }

    [Inject]
    private void Construct(TechTreeBonusService techTreeBonusService)
    {
        _techTreeBonusService = techTreeBonusService;
    }

    public void Register(ArmyUnit unit)
    {
        if (_allUnits.Contains(unit))
            return;

        _allUnits.Add(unit);

        if (unit.IsPlayerUnit() && _reservedPlayerArmySlots > 0)
            _reservedPlayerArmySlots--;

        OnArmyChanged?.Invoke();
    }

    public void Unregister(ArmyUnit unit)
    {
        if (!_allUnits.Remove(unit))
            return;

        OnArmyChanged?.Invoke();
    }

    public bool HasFreePlayerSlot()
    {
        return FreePlayerArmySlots > 0;
    }

    public bool TryReservePlayerSlot()
    {
        if (!HasFreePlayerSlot())
            return false;

        _reservedPlayerArmySlots++;
        OnArmyChanged?.Invoke();

        return true;
    }

    public void ReleasePlayerSlotReservation()
    {
        if (_reservedPlayerArmySlots <= 0)
            return;

        _reservedPlayerArmySlots--;
        OnArmyChanged?.Invoke();
    }

    public void ReleasePlayerSlotReservations(int count)
    {
        if (count <= 0)
            return;

        int oldValue = _reservedPlayerArmySlots;

        _reservedPlayerArmySlots = Mathf.Max(
            0,
            _reservedPlayerArmySlots - count);

        if (_reservedPlayerArmySlots != oldValue)
            OnArmyChanged?.Invoke();
    }

    public void GetAllPlayerUnitsNonAlloc(List<ArmyUnit> result)
    {
        if (result == null)
            return;

        result.Clear();

        for (int i = 0; i < _allUnits.Count; i++)
        {
            ArmyUnit unit = _allUnits[i];

            if (unit == null)
                continue;

            if (!unit.IsPlayerUnit())
                continue;

            result.Add(unit);
        }
    }

    private int CountPlayerUnits()
    {
        int count = 0;

        for (int i = 0; i < _allUnits.Count; i++)
        {
            ArmyUnit unit = _allUnits[i];

            if (unit == null)
                continue;

            if (unit.IsPlayerUnit())
                count++;
        }

        return count;
    }

    private void OnDestroy()
    {
        _allUnits.Clear();
        _reservedPlayerArmySlots = 0;
        OnArmyChanged = null;
    }
    private int GetArmyCapBonus()
    {
        return _techTreeBonusService.GetBonusInt(TechTreeBonusType.ArmyCap);
    }
}