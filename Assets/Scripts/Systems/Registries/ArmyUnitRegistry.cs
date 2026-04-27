using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех боевых юнитов на сцене
/// </summary>
public class ArmyUnitRegistry : MonoBehaviour
{
    [SerializeField] private int _maxPlayerArmyUnits = 10;

    private readonly List<ArmyUnit> _allUnits = new();

    public IReadOnlyList<ArmyUnit> AllUnits => _allUnits;
    public int MaxPlayerArmyUnits => _maxPlayerArmyUnits;
    public int CurrentPlayerArmyUnits => CountPlayerUnits();

    public event Action OnArmyChanged;

    private void OnValidate()
    {
        _maxPlayerArmyUnits = Mathf.Max(1, _maxPlayerArmyUnits);
    }

    public void Register(ArmyUnit unit)
    {
        if (unit == null)
            return;

        if (_allUnits.Contains(unit))
            return;

        _allUnits.Add(unit);
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
        return CurrentPlayerArmyUnits < _maxPlayerArmyUnits;
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
