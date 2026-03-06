using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMovement))]
public class SheepAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float eatTime = 3f;

    [Header("Territory")]
    [SerializeField] private SheepTerritory territory;

    private UnitMovement movement;
    private Animator animator;

    private float timer;
    private bool isEating;
    private bool frozen;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        animator = GetComponent<Animator>();

        if (territory == null)
        {
            Debug.LogError($"Sheep {name} has no territory assigned!");
        }
    }

    private void Start()
    {
        GoToRandomPoint();
    }

    private void Update()
    {
        if (frozen)
            return;

        animator.SetBool("IsMoving", movement.HasTarget);

        if (!movement.HasTarget && !isEating)
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
                GoToRandomPoint();
            }
        }
    }

    public void SetFrozen(bool value)
    {
        frozen = value;

        if (value)
        {
            movement.Stop();
        }
        else
        {
            GoToRandomPoint();
        }
    }

    private void GoToRandomPoint()
    {
        if (territory == null)
            return;

        Vector2 point = territory.GetRandomPoint();
        movement.MoveTo(point);
    }
}
