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

    public void PlayWork(WorkerJobType job)
    {
        if (job == WorkerJobType.ChopWood)
            animator.SetTrigger("Chop");
        else if (job == WorkerJobType.MineGold)
            animator.SetTrigger("Mine");
    }

    public void SetTool(WorkerJobType job)
    {
        animator.SetBool("HasAxe", job == WorkerJobType.ChopWood);
        animator.SetBool("HasPickaxe", job == WorkerJobType.MineGold);
    }

    public void SetCarry(WorkerJobType job)
    {
        animator.SetBool("HasWood", job == WorkerJobType.ChopWood);
        animator.SetBool("HasGold", job == WorkerJobType.MineGold);
    }

    public void ClearCarry()
    {
        animator.SetBool("HasWood", false);
        animator.SetBool("HasGold", false);
    }

    public void ClearTool()
    {
        animator.SetBool("HasAxe", false);
        animator.SetBool("HasPickaxe", false);
    }

    public void SetIdle()
    {
        ClearCarry();
        ClearTool();
    }
}

