using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// перемещение по случайным точкам внутри территории 
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
    private bool isfrozen;
    private Vector2 targetPoint;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        eatTime = Mathf.Max(0.1f, eatTime);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => agent != null && agent.isOnNavMesh);
        MoveToNewPoint();
    }

    private void Update()
    {
        if (isfrozen)
            return;

        if (animator != null)
            animator.SetBool("IsMoving", movement != null && movement.HasTarget);

        if (!isEating && IsAtTarget())
        {
            isEating = true;
            timer = eatTime;
        }

        if (!isEating)
            return;

        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        isEating = false;
        MoveToNewPoint();
    }

    public void SetFrozen(bool value)
    {
        if (isfrozen == value)
            return;

        isfrozen = value;

        if (value)
        {
            movement?.Stop();

            if (animator != null)
                animator.SetBool("IsMoving", false);

            return;
        }

        MoveToNewPoint();
    }

    private bool IsAtTarget()
    {
        return Vector2.Distance(transform.position, targetPoint) < 0.2f;
    }

    private void MoveToNewPoint()
    {
        if (territory == null || movement == null)
            return;

        for (int i = 0; i < 5; i++)
        {
            Vector2 random = territory.GetRandomPoint();

            if (!NavMesh.SamplePosition(random, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                continue;

            targetPoint = hit.position;
            movement.MoveTo(hit.position);
            return;
        }

        Debug.LogWarning($"{name}: no valid NavMesh point in territory.", this);
    }
}
