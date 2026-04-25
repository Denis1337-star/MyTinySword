using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех боевых юнитов на сцене
/// Контроль общего лимита армии
/// выбор всех союзных юнитов 
/// </summary>
public class ArmyUnitRegistry : MonoBehaviour
{
    public static ArmyUnitRegistry Instance { get; private set; }

    [Header("Limits")]
    [SerializeField] private int maxPlayerArmyUnits = 10;

    private readonly List<ArmyUnit> allUnits = new();

    public IReadOnlyList<ArmyUnit> AllUnits => allUnits;
    public int MaxPlayerArmyUnits => maxPlayerArmyUnits;
    public int CurrentPlayerArmyUnits => CountPlayerUnits();

    public event Action OnArmyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        maxPlayerArmyUnits = Mathf.Max(1, maxPlayerArmyUnits);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnValidate()
    {
        maxPlayerArmyUnits = Mathf.Max(1, maxPlayerArmyUnits);
    }

    public void Register(ArmyUnit unit)
    {
        if (unit == null)
            return;

        if (allUnits.Contains(unit))
            return;

        allUnits.Add(unit);
        OnArmyChanged?.Invoke();
    }

    public void Unregister(ArmyUnit unit)
    {
        if (unit == null)
            return;

        if (!allUnits.Remove(unit))
            return;

        OnArmyChanged?.Invoke();
    }

    public bool HasFreePlayerSlot()
    {
        return CurrentPlayerArmyUnits < maxPlayerArmyUnits;
    }

    public List<ArmyUnit> GetAllPlayerUnits()
    {
        List<ArmyUnit> result = new();

        foreach (ArmyUnit unit in allUnits)
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

        foreach (ArmyUnit unit in allUnits)
        {
            if (unit == null)
                continue;

            if (unit.IsPlayerUnit())
                count++;
        }

        return count;
    }
}
