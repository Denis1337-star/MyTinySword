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
                animator.SetFloat("Equipment", 1);
               // animator.SetBool("HasAxe", true);
                break;
            case EquipmentType.Pickaxe:
                animator.SetFloat("Equipment", 2);
               // animator.SetBool("HasPickaxe", true);
                break;
            case EquipmentType.Knife:
                animator.SetFloat("Equipment", 3);
               // animator.SetBool("HasKnife", true);
                break;

            case EquipmentType.Wood:
                animator.SetFloat("Equipment", 4);
               // animator.SetBool("HasWood", true);
                break;
            case EquipmentType.Gold:
                animator.SetFloat("Equipment", 5);
              //  animator.SetBool("HasGold", true);
                break;
            case EquipmentType.Meat:
                animator.SetFloat("Equipment", 6);
               // animator.SetBool("HasMeat", true);
                break;
            case EquipmentType.None:
                animator.SetFloat("Equipment", 0);
                // animator.SetBool("HasWood", true);
                break;
        }
    }

    private void ResetEquipment()
    {
        animator.SetFloat("Equipment", 0);
        //animator.SetBool("HasAxe", false);
        //animator.SetBool("HasPickaxe", false);
        //animator.SetBool("HasKnife", false);

        //animator.SetBool("HasWood", false);
        //animator.SetBool("HasGold", false);
        //animator.SetBool("HasMeat", false);
    }
}

