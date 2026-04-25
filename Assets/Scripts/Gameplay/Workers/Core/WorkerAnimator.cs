using UnityEngine;

public class WorkerAnimator : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsWorkingHash = Animator.StringToHash("IsWorking");
    private static readonly int EquipmentHash = Animator.StringToHash("Equipment");

    [SerializeField] private Animator animator;
    [SerializeField] private UnitMovement movement;

    private bool isWorking;
    private bool lastIsMoving;
    private EquipmentType currentEquipment = EquipmentType.None;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        bool isMoving = movement != null && movement.IsMoving && !isWorking;

        if (lastIsMoving == isMoving)
            return;

        lastIsMoving = isMoving;
        animator.SetBool(IsMovingHash, isMoving);
    }

    public void SetWorking(bool value)
    {
        if (isWorking == value)
            return;

        isWorking = value;
        animator.SetBool(IsWorkingHash, value);

        if (value)
        {
            lastIsMoving = false;
            animator.SetBool(IsMovingHash, false);
        }
    }

    public void SetEquipment(EquipmentType equipment)
    {
        if (currentEquipment == equipment)
            return;

        currentEquipment = equipment;
        animator.SetFloat(EquipmentHash, (float)equipment);
    }

    private void ResolveReferences()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movement == null)
            movement = GetComponent<UnitMovement>();
    }
}

