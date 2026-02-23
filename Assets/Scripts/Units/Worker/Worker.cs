using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WorkerState
{
    Idle,
    GoingToResource,
    Working,
    CarryingToHouse
}

[RequireComponent(typeof(UnitMovement))]
public class Worker : MonoBehaviour
{
    [SerializeField] private WorkerTaskUI taskUI;

    private WorkerState state = WorkerState.Idle;
    public WorkerState CurrentState => state;

    private UnitMovement movement;
    private WorkerAnimator animator;

    private IResourceNode targetResource;
    private House targetHouse;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        animator = GetComponent<WorkerAnimator>();

        if (taskUI != null)
            taskUI.Initialize(this);
    }

    private void Update()
    {
        switch (state)
        {
            case WorkerState.GoingToResource:
                if (!movement.HasTarget)
                {
                    StartWorking();
                }
                break;

            case WorkerState.CarryingToHouse:
                if (!movement.HasTarget)
                {
                    FinishDelivery();
                }
                break;
        }
    }

    // === ¬€«€¬¿≈“—ﬂ »« UI ===
    public void AssignChopWoodTask()
    {
        if (state != WorkerState.Idle)
            return;

        targetResource = ResourceFinder.FindBest<TreeResource>(transform.position);
        if (targetResource == null)
            return;

        animator.SetAxe(true);
        animator.SetCarry(false);

        state = WorkerState.GoingToResource;
        movement.MoveTo(targetResource.WorkPosition);
    }

    private void StartWorking()
    {
        state = WorkerState.Working;
        animator.PlayChop();

        targetResource.StartWork(OnWorkFinished);
    }

    private void OnWorkFinished()
    {
        targetHouse = FindObjectOfType<House>();
        if (targetHouse == null)
            return;

        animator.SetCarry(true);
        animator.SetAxe(false);

        state = WorkerState.CarryingToHouse;
        movement.MoveTo(targetHouse.DropPoint);
    }

    private void FinishDelivery()
    {
        ResourceStorage.Instance.AddWood((int)targetResource.Size);

        animator.SetCarry(false);

        state = WorkerState.Idle;

        //  ¿¬“Œ÷» À
        AssignChopWoodTask();
    }
    public void AssignMineGoldTask()
    {
        if (state != WorkerState.Idle)
            return;

        targetResource = ResourceFinder.FindBest<GoldResource>(transform.position);
        if (targetResource == null)
            return;

        animator.SetAxe(false);
        animator.SetCarry(false);

        state = WorkerState.GoingToResource;
        movement.MoveTo(targetResource.WorkPosition);
    }
}