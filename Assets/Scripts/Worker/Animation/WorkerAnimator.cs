using UnityEngine;

public enum WorkerAction
{
    None,
    Move,
    Work,
    Carry,
    Idle
}
public enum EquipmentType
{
    None,
    Axe,
    Pickaxe,
    Knife,
    Wood,
    Gold,
    Meat
}
public class WorkerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnitMovement movement;

    private void Update()
    {
        animator.SetBool("IsMoving", movement.HasTarget);
    }

    public void PlayAction(WorkerAction action)
    {
        switch (action)
        {
            case WorkerAction.Work:
                animator.SetTrigger("Work");
                break;

            case WorkerAction.Idle:
                animator.SetTrigger("Idle");
                break;
        }
    }

    public void SetEquipment(EquipmentType equipment)
    {
        ResetEquipment();

        switch (equipment)
        {
            case EquipmentType.Axe:
                animator.SetBool("HasAxe", true);
                break;
            case EquipmentType.Pickaxe:
                animator.SetBool("HasPickaxe", true);
                break;
            case EquipmentType.Knife:
                animator.SetBool("HasKnife", true);
                break;

            case EquipmentType.Wood:
                animator.SetBool("HasWood", true);
                break;
            case EquipmentType.Gold:
                animator.SetBool("HasGold", true);
                break;
            case EquipmentType.Meat:
                animator.SetBool("HasMeat", true);
                break;
        }
    }

    private void ResetEquipment()
    {
        animator.SetBool("HasAxe", false);
        animator.SetBool("HasPickaxe", false);
        animator.SetBool("HasKnife", false);

        animator.SetBool("HasWood", false);
        animator.SetBool("HasGold", false);
        animator.SetBool("HasMeat", false);
    }
}

