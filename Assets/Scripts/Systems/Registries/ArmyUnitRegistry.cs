using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех боевых юнитов на сцене.
/// Также хранит зарезервированные места для юнитов, которые уже стоят в очереди производства.
/// </summary>
public sealed class ArmyUnitRegistry : MonoBehaviour
{
    [Header("Army Limit")]
    [SerializeField] private int _maxPlayerArmyUnits = 10;

    private readonly List<ArmyUnit> _allUnits = new();

    private int _reservedPlayerArmySlots;

    public IReadOnlyList<ArmyUnit> AllUnits => _allUnits;

    public int MaxPlayerArmyUnits => _maxPlayerArmyUnits;

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
        _reservedPlayerArmySlots = Mathf.Max(0, _reservedPlayerArmySlots);
    }

    public void Register(ArmyUnit unit)
    {
        if (unit == null)
            return;

        if (_allUnits.Contains(unit))
            return;

        _allUnits.Add(unit);

        // Если юнит был создан из очереди производства,
        // его место уже было зарезервировано раньше.
        // Теперь резерв превращается в реального живого юнита.
        if (unit.IsPlayerUnit() && _reservedPlayerArmySlots > 0)
            _reservedPlayerArmySlots--;

        OnArmyChanged?.Invoke();
    }

    public void Unregister(ArmyUnit unit)
    {
        if (unit == null)
            return;

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

    public List<ArmyUnit> GetAllPlayerUnits()
    {
        List<ArmyUnit> result = new();

        foreach (ArmyUnit unit in _allUnits)
        {
            if (unit == null)
                continue;

            if (!unit.IsPlayerUnit())
                continue;

            result.Add(unit);
        }

        return result;
    }

    private int CountPlayerUnits()
    {
        int count = 0;

        foreach (ArmyUnit unit in _allUnits)
        {
            if (unit == null)
                continue;

            if (unit.IsPlayerUnit())
                count++;
        }

        return count;
    }
}