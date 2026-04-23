using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый компонент боевого юнита.
/// Пока отвечает только за принадлежность к армии игрока/врага
/// и регистрацию в ArmyUnitRegistry.
/// </summary>
public class ArmyUnit : MonoBehaviour
{
    [SerializeField] private FactionMember factionMember;
    [SerializeField] private UnitConfig config;

    public FactionMember FactionMember => factionMember;
    public UnitConfig Config => config;

    private void Awake()
    {
        if (factionMember == null)
            factionMember = GetComponent<FactionMember>();
    }

    private void Start()
    {
        ArmyUnitRegistry.Instance?.Register(this);
    }

    private void OnDestroy()
    {
        ArmyUnitRegistry.Instance?.Unregister(this);
    }

    /// <summary>
    /// Проверяет, принадлежит ли юнит игроку.
    /// </summary>
    public bool IsPlayerUnit()
    {
        return factionMember != null && factionMember.Faction == FactionType.Player;
    }
}
