using System;
using System.Collections;
using UnityEngine;



public enum WorkerJobType
{
    None,
    ChopWood,
    MineGold,
    HuntMeat
}


[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(WorkerInventory))]
[RequireComponent(typeof(WorkerBrain))]
public class Worker : MonoBehaviour
{
    public WorkerStateMachine StateMachine { get; private set; }
    public UnitMovement Movement { get; private set; }
    public WorkerAnimator Animator { get; private set; }
    public WorkerInventory Inventory { get; private set; }
    public WorkerBrain Brain { get; private set; }

    public House Home { get; private set; }
    public IWorkerJob CurrentJobLogic { get; private set; }
    public WorkerJobType CurrentJob { get; private set; }
    public WorkerJobType PendingJob { get; private set; } = WorkerJobType.None;

    public ResourceNodeBase TargetResource { get; set; }
    public WorkSlot TargetSlot { get; set; }

    public event Action OnJobChanged;
    public event Action OnActivityChanged;

    private void Awake()
    {
        Movement = GetComponent<UnitMovement>();
        Animator = GetComponent<WorkerAnimator>();
        Inventory = GetComponent<WorkerInventory>();
        Brain = GetComponent<WorkerBrain>();

        StateMachine = new WorkerStateMachine();
        CurrentJob = WorkerJobType.None;
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void OnDestroy()
    {
        if (TargetResource != null)
            TargetResource.CancelWork(this);

        WorkerRegistry.Instance?.Unregister(this);
    }

    public void SetHome(House house)
    {
        Home = house;
        WorkerRegistry.Instance.Register(this);
        ChangeState(new WorkerIdleState(this));
    }

    public void AssignJob(WorkerJobType job)
    {
        Brain.AssignJob(job);
    }

    public void SetCurrentJob(WorkerJobType job, IWorkerJob logic)
    {
        CurrentJob = job;
        CurrentJobLogic = logic;
        OnJobChanged?.Invoke();
    }

    public void SetPendingJob(WorkerJobType job)
    {
        PendingJob = job;
        OnJobChanged?.Invoke();
    }

    public void ClearPendingJob()
    {
        PendingJob = WorkerJobType.None;
        OnJobChanged?.Invoke();
    }

    public void ChangeState(IWorkerState state)
    {
        StateMachine.ChangeState(state);
        OnActivityChanged?.Invoke();
    }
}
