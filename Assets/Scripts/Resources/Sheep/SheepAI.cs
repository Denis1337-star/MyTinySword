using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Простое AI-поведение овцы
/// перемещение по случайным точкам внутри территории и паузы на месте
/// Может быть временно заморожено ресурсной системой
/// </summary>
[RequireComponent(typeof(UnitMovement))]
public class SheepAI : MonoBehaviour
{
    [SerializeField] private float eatTime = 3f;
    [SerializeField] private SheepTerritory territory;

    private UnitMovement movement;
    private Animator animator;
    private NavMeshAgent agent;

    private float timer;
    private bool isEating;
    private bool frozen;
    private Vector2 targetPoint;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => agent != null && agent.isOnNavMesh);
        MoveToNewPoint();
    }

    private void Update()
    {
        if (frozen)
            return;

        animator.SetBool("IsMoving", movement.HasTarget);

        if (!isEating && IsAtTarget())
        {
            isEating = true;
            timer = eatTime;
        }

        if (isEating)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                isEating = false;
                MoveToNewPoint();
            }
        }
    }

    /// <summary>
    /// Проверяет, дошла ли овца до текущей цели
    /// </summary>
    private bool IsAtTarget()
    {
        return Vector2.Distance(transform.position, targetPoint) < 0.2f;
    }
    /// <summary>
    /// Выбирает новую случайную точку внутри территории и отправляет овцу к ней
    /// </summary>
    private void MoveToNewPoint()
    {
        if (territory == null)
            return;

        for (int i = 0; i < 5; i++) // 5 попыток найти точку
        {
            Vector2 random = territory.GetRandomPoint();

            if (NavMesh.SamplePosition(random, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                targetPoint = hit.position;
                movement.MoveTo(hit.position);
                return;
            }
        }

        Debug.LogWarning("Sheep: no valid NavMesh point in territory");
    }

    /// <summary>
    /// Замораживает или размораживает поведение овцы
    /// </summary>
    public void SetFrozen(bool value)
    {
        frozen = value;

        if (value)
            movement.Stop();
        else
            MoveToNewPoint();
    }
}
