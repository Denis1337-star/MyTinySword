using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Связывает gameplay-логику юнита с Animator.
/// Отвечает за параметры движения и атаки.
/// </summary>
public class UnitAnimatorBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnitMovement movement;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movement == null)
            movement = GetComponent<UnitMovement>();
    }

    private void Update()
    {
        if (animator == null || movement == null)
            return;

        animator.SetBool(IsMovingHash, movement.IsMoving);
    }

    /// <summary>
    /// Запускает анимацию атаки.
    /// </summary>
    public void PlayAttack()
    {
        if (animator == null)
            return;

        animator.SetTrigger(AttackHash);
    }
}
