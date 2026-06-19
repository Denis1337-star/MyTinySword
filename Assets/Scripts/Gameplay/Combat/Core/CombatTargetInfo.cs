using UnityEngine;

/// <summary>
/// Описывает боевую цель 
/// </summary>
public class CombatTargetInfo : MonoBehaviour
{
    [SerializeField] private TargetPriorityType targetPriority = TargetPriorityType.Building;

    public TargetPriorityType TargetPriority => targetPriority;
}
