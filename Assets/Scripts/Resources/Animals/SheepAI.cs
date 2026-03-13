using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitMovement))]
public class SheepAI : MonoBehaviour
{
    [SerializeField] private float eatTime = 3f;
    [SerializeField] private SheepTerritory territory;

    private UnitMovement movement;
    private Animator animator;

    private float timer;
    private bool isEating;
    private bool frozen;
    private Vector2 targetPoint;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        animator = GetComponent<Animator>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            GetComponent<NavMeshAgent>().isOnNavMesh
        );

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

    private bool IsAtTarget()
    {
        return Vector2.Distance(transform.position, targetPoint) < 0.2f;
    }

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

    public void SetFrozen(bool value)
    {
        frozen = value;

        if (value)
            movement.Stop();
        else
            MoveToNewPoint();
    }
}
