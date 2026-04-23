using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Глобальный реестр всех боевых юнитов на сцене.
/// Нужен для контроля общего лимита армии,
/// выбора всех союзных юнитов и будущей боевой логики.
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

    private void OnValidate()
    {
        maxPlayerArmyUnits = Mathf.Max(1, maxPlayerArmyUnits);
    }

    /// <summary>
    /// Регистрирует боевого юнита.
    /// </summary>
    public void Register(ArmyUnit unit)
    {
        if (unit == null)
            return;

        if (allUnits.Contains(unit))
            return;

        allUnits.Add(unit);
        OnArmyChanged?.Invoke();
    }

    /// <summary>
    /// Удаляет боевого юнита из реестра.
    /// </summary>
    public void Unregister(ArmyUnit unit)
    {
        if (unit == null)
            return;

        if (!allUnits.Remove(unit))
            return;

        OnArmyChanged?.Invoke();
    }

    /// <summary>
    /// Проверяет, есть ли свободное место в армии игрока.
    /// </summary>
    public bool HasFreePlayerSlot()
    {
        return CurrentPlayerArmyUnits < maxPlayerArmyUnits;
    }

    /// <summary>
    /// Возвращает всех живых союзных боевых юнитов игрока.
    /// </summary>
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
