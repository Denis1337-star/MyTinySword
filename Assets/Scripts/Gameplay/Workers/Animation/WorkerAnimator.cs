using UnityEngine;


/// <summary>
/// Отвечает за перевод состояния worker'а в параметры Animator
/// </summary>
public class WorkerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private UnitMovement movement;

    private bool isWorking;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    /// <summary>
    /// Пытается автоматически найти нужные ссылки на том же объекте
    /// </summary>
    private void ResolveReferences()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movement == null)
            movement = GetComponent<UnitMovement>();
    }

    private void Update()
    {
        if (animator == null)
            return;

        bool isMoving = movement != null && movement.IsMoving && !isWorking;
        animator.SetBool("IsMoving", isMoving);
    }

    /// <summary>
    /// Включает или выключает рабочую анимацию
    /// </summary>
    public void SetWorking(bool value)
    {
        if (animator == null)
            return;

        if (isWorking == value)
            return;

        isWorking = value;
        animator.SetBool("IsWorking", value);

        if (value)
            animator.SetBool("IsMoving", false);
    }
    /// <summary>
    /// Обновляет визуальный тип инструмента/груза в руках worker'а
    /// </summary>
    public void SetEquipment(EquipmentType equipment)
    {
        if (animator == null)
            return;

        animator.SetFloat("Equipment", (float)equipment);
    }
}

