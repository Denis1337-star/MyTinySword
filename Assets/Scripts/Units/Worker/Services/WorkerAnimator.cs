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
        animator.SetBool("IsMoving", movement != null && movement.HasTarget);
    }

    public void SetWorking(bool value)
    {
        Debug.Log($"[WorkerAnimator] SetWorking({value}) on {name}", this);
        animator.SetBool("IsWorking", value);
    }

    public void SetEquipment(EquipmentType equipment)
    {
        animator.SetFloat("Equipment", (float)equipment);
    }
}

