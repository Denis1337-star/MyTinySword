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

    private IWorkerJob currentJobLogic;

    private UnitMovement movement;
    private WorkerAnimator animator;

    private ResourceNodeBase targetResource;
    private WorkSlot targetSlot;
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
        WorkerRegistry.Instance?.Unregister(this);
    }

    private void Update()
    {
        switch (state)
        {
            case WorkerState.GoingToResource:
                if (IsAtWorkSlot())
                {
                    if (targetSlot != null)
                    {
                        SetState(WorkerState.Working);
                        animator.PlayAction(WorkerAction.Work);
                        targetResource.StartWork(OnResourceFinished);
                    }
                    else
                    {
                        TryFindNextResource();
                    }
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
        currentJobLogic = WorkerJobFactory.Create(job);
        queuedJob = WorkerJobType.None;

        OnJobChanged?.Invoke(this);
        TryFindNextResource();
    }

    private void TryFindNextResource()
    {
        ReleaseResource();

        targetResource = currentJobLogic?.FindResource(transform.position);

        if (targetResource == null)
        {
            SetIdle();
            return;
        }

        targetSlot = targetResource.GetFreeSlot(this);
        if (targetSlot == null)
        {
            SetIdle();
            return;
        }

        SetState(WorkerState.GoingToResource);
        animator.SetEquipment(GetToolForJob());
        movement.MoveTo(targetSlot.Position);
    }

    private void OnResourceFinished(int amount)
    {
        carriedAmount = amount;
        ReleaseResource();

        animator.SetEquipment(GetCarryForJob());
        SetState(WorkerState.CarryingToHouse);
        movement.MoveTo(targetHouse.DropPoint);
    }

    private IEnumerator UnloadRoutine()
    {
        animator.PlayAction(WorkerAction.Idle);
        animator.SetEquipment(EquipmentType.None);

        yield return new WaitForSeconds(unloadDuration);

        currentJobLogic?.GiveReward(carriedAmount);
        carriedAmount = 0;

        if (queuedJob != WorkerJobType.None)
            StartNewJob(queuedJob);
        else
            TryFindNextResource();
    }
    private void ReleaseResource()
    {
        if (targetResource != null)
            targetResource.ReleaseSlot(this);

        targetResource = null;
        targetSlot = null;
    }

    private void SetState(WorkerState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(this);
    }
    private void SetIdle()
    {
        SetState(WorkerState.Idle);
        animator.PlayAction(WorkerAction.Idle);
        animator.SetEquipment(EquipmentType.None);
        movement.MoveTo(targetHouse.GetIdlePosition(this));
    }
    public void SetHome(House house)
    {
        targetHouse = house;
    }
    private bool IsAtWorkSlot()
    {
        if (targetSlot == null)
            return false;

        return Vector2.Distance(transform.position, targetSlot.Position) <= 0.15f;
    }
    private EquipmentType GetToolForJob()
    {
        return CurrentJob switch
        {
            WorkerJobType.ChopWood => EquipmentType.Axe,
            WorkerJobType.MineGold => EquipmentType.Pickaxe,
            WorkerJobType.HuntMeat => EquipmentType.Knife,
            _ => EquipmentType.None
        };
    }

    private EquipmentType GetCarryForJob()
    {
        return CurrentJob switch
        {
            WorkerJobType.ChopWood => EquipmentType.Wood,
            WorkerJobType.MineGold => EquipmentType.Gold,
            WorkerJobType.HuntMeat => EquipmentType.Meat,
            _ => EquipmentType.None
        };
    }
}
