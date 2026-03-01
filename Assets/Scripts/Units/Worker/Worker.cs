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

    private IResourceNode targetResource;
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
                    SetState(WorkerState.Working);
                    animator.PlayWork(CurrentJob);

                    StartResourceWork();
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
        CurrentJob = job;
        queuedJob = WorkerJobType.None;
        OnJobChanged?.Invoke(this);

        TryFindNextResource();
    }

    private void TryFindNextResource()
    {
        targetResource = FindResourceForJob();

        if (targetResource == null)
        {
            state = WorkerState.Idle;
            animator.SetIdle();
            return;
        }

        SetState(WorkerState.GoingToResource);
        animator.SetTool(CurrentJob);
        movement.MoveTo(targetResource.WorkPosition);
    }

    private void StartResourceWork()
    {
        targetResource.StartWork(OnResourceFinished);
    }

    private void OnResourceFinished(int amount)
    {
        carriedAmount = amount;

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
        {
            StartNewJob(queuedJob);
        }
        else
        {
            TryFindNextResource();
        }
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

    private void SetState(WorkerState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(this);
    }

    private IResourceNode FindResourceForJob()
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