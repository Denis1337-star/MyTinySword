using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnitMovement movement;

    private void Update()
    {
        animator.SetBool("IsMoving", movement.HasTarget);
    }

    public void SetAxe(bool value)
    {
        animator.SetBool("HasAxe", value);
    }

    public void SetCarry(bool value)
    {
        animator.SetBool("HasWood", value);
    }

    public void PlayChop()
    {
        animator.SetTrigger("Chop");
    }
}
