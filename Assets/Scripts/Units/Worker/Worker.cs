using System;
using System.Collections;
using UnityEngine;


public enum WorkerState
{
    Idle,               // стоит, ждёт приказа
    GoingToResource,    // идёт к ресурсу
    Working,            // работает (рубит / добывает)
    CarryingToHouse,     // несёт ресурсы домой
    Unloading
}
public enum WorkerJobType
{
    None,
    ChopWood,
    MineGold,
    HuntMeat
}


[RequireComponent(typeof(UnitMovement))]
public class Worker : MonoBehaviour
{
    public event Action<Worker> OnStateChanged;
    public event Action<Worker> OnJobChanged;

    [SerializeField] private float unloadDuration = 2f;

    private WorkerState state = WorkerState.Idle;
    public WorkerState CurrentState => state;

    public WorkerJobType CurrentJob { get; private set; } = WorkerJobType.None;
    private WorkerJobType queuedJob = WorkerJobType.None;

    private UnitMovement movement;
    private WorkerAnimator animator;

    private ResourceNodeBase targetResource;
    private House targetHouse;

    private int carriedAmount;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        animator = GetComponent<WorkerAnimator>();
        targetHouse = FindObjectOfType<House>();
    }

    private void Start()
    {
        WorkerRegistry.Instance.Register(this);
    }

    private void OnDestroy()
    {
        WorkerRegistry.Instance.Unregister(this);
    }

    private void Update()
    {
        switch (state)
        {
            case WorkerState.GoingToResource:
                if (!movement.HasTarget)
                {
                    if (targetResource == null || !targetResource.IsAvailable)
                    {
                        TryFindNextResource();
                        return;
                    }

                    SetState(WorkerState.Working);
                    animator.PlayWork(CurrentJob);
                    targetResource.StartWork(OnResourceFinished);
                }
                break;

            case WorkerState.CarryingToHouse:
                if (!movement.HasTarget)
                {
                    SetState(WorkerState.Unloading);
                    StartCoroutine(UnloadRoutine());
                }
                break;
        }
    }

    public void AssignJob(WorkerJobType job)
    {
        if (CurrentJob == WorkerJobType.None || state == WorkerState.Idle)
        {
            StartNewJob(job);
        }
        else
        {
            queuedJob = job;
        }
    }

    private void StartNewJob(WorkerJobType job)
    {
        ReleaseResource();

        CurrentJob = job;
        queuedJob = WorkerJobType.None;
        OnJobChanged?.Invoke(this);

        TryFindNextResource();
    }

    private void TryFindNextResource()
    {
        ReleaseResource();

        targetResource = FindAndReserveResource();

        if (targetResource == null)
        {
            SetState(WorkerState.Idle);
            animator.SetIdle();
            return;
        }

        SetState(WorkerState.GoingToResource);
        animator.SetTool(CurrentJob);
        movement.MoveTo(targetResource.WorkPosition);
    }

    private ResourceNodeBase FindAndReserveResource()
    {
        ResourceNodeBase resource = FindResourceForJob();

        if (resource == null)
            return null;

        if (!resource.TryReserve(this))
            return null;

        return resource;
    }

    private void OnResourceFinished(int amount)
    {
        carriedAmount = amount;

        ReleaseResource();

        animator.SetCarry(CurrentJob);
        SetState(WorkerState.CarryingToHouse);
        movement.MoveTo(targetHouse.DropPoint);
    }

    private IEnumerator UnloadRoutine()
    {
        animator.SetIdle();

        yield return new WaitForSeconds(unloadDuration);

        GiveResources();
        carriedAmount = 0;

        if (queuedJob != WorkerJobType.None)
            StartNewJob(queuedJob);
        else
            TryFindNextResource();
    }

    private void GiveResources()
    {
        switch (CurrentJob)
        {
            case WorkerJobType.ChopWood:
                ResourceStorage.Instance.AddWood(carriedAmount);
                break;
            case WorkerJobType.MineGold:
                ResourceStorage.Instance.AddGold(carriedAmount);
                break;
            case WorkerJobType.HuntMeat:
                ResourceStorage.Instance.AddMeat(carriedAmount);
                break;
        }
    }

    private void ReleaseResource()
    {
        if (targetResource != null)
        {
            targetResource.Release(this);
            targetResource = null;
        }
    }

    private void SetState(WorkerState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(this);
    }

    private ResourceNodeBase FindResourceForJob()
    {
        return CurrentJob switch
        {
            WorkerJobType.ChopWood => ResourceFinder.FindBest<TreeResource>(transform.position),
            WorkerJobType.MineGold => ResourceFinder.FindBest<GoldResource>(transform.position),
            WorkerJobType.HuntMeat => ResourceFinder.FindBest<SheepResource>(transform.position),
            _ => null
        };
    }
}