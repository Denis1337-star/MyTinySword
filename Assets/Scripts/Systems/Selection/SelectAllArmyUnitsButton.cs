using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
///  нопка выбора всех боевых юнитов игрока
/// </summary>
[RequireComponent(typeof(Button))]
public class SelectAllArmyUnitsButton : MonoBehaviour
{
    private Button _button;
    private SelectionSystem _selectionSystem;
    private ArmyUnitRegistry _armyUnitRegistry;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        ArmyUnitRegistry armyUnitRegistry)
    {
        _selectionSystem = selectionSystem;
        _armyUnitRegistry = armyUnitRegistry;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button != null)
            _button.onClick.AddListener(SelectAllPlayerUnits);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(SelectAllPlayerUnits);
    }

    private void SelectAllPlayerUnits()
    {
        if (_selectionSystem == null || _armyUnitRegistry == null)
            return;

        List<ArmyUnit> units = _armyUnitRegistry.GetAllPlayerUnits();

        _selectionSystem.SelectArmyUnits(units);
    }
}
