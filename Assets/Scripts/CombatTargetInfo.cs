
using UnityEngine;

/// <summary>
/// Описывает боевую цель для системы выбора цели.
/// Помогает юнитам понимать, что перед ними:
/// боевой юнит, башня, обычное здание или castle.
/// </summary>
public class CombatTargetInfo : MonoBehaviour
{
    [SerializeField] private TargetPriorityType targetPriority = TargetPriorityType.Building;

    public TargetPriorityType TargetPriority => targetPriority;
}
