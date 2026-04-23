using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Сервис выбора всех союзных боевых юнитов игрока.
/// Вызывается из кнопки на панели армии.
/// </summary>
public class SelectAllArmyUnitsButton : MonoBehaviour
{
    public static SelectAllArmyUnitsButton Instance { get; private set; }

    [SerializeField] private SelectionSystem selectionSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (selectionSystem == null)
            selectionSystem = FindObjectOfType<SelectionSystem>(true);
    }

    /// <summary>
    /// Выбирает всех боевых юнитов игрока.
    /// </summary>
    public void SelectAllPlayerUnits()
    {
        if (selectionSystem == null || ArmyUnitRegistry.Instance == null)
            return;

        List<ArmyUnit> units = ArmyUnitRegistry.Instance.GetAllPlayerUnits();
        selectionSystem.SelectArmyUnits(units);
    }
}
