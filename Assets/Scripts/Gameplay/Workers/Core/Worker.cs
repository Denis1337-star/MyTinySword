using System;
using UnityEngine;

/// <summary>
/// Центральная сущность worker'а
/// Объединяет state machine, brain, inventory, movement и текущую job-логику
/// </summary>
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(WorkerInventory))]
[RequireComponent(typeof(WorkerBrain))]
public class Worker : ValidatedMonoBehaviour
{
    [Header("Config")]
    [SerializeField] private WorkerConfig config;

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

    public WorkerConfig Config => config;

    public event Action OnJobChanged;
    public event Action OnActivityChanged;

    public string CurrentStateName => StateMachine != null ? StateMachine.CurrentStateName : "None";
    public bool HasCargo => Inventory != null && Inventory.HasCargo;
    public bool HasPendingJob => PendingJob != WorkerJobType.None;

    private WorkerIdleState idleState;
    private WorkerFindResourceState findResourceState;
    private WorkerGoToResourceState goToResourceState;
    private WorkerWorkState workState;
    private WorkerCarryState carryState;

    protected override void Awake()
    {
        Movement = GetComponent<UnitMovement>();
        Animator = GetComponent<WorkerAnimator>();
        Inventory = GetComponent<WorkerInventory>();
        Brain = GetComponent<WorkerBrain>();

        StateMachine = new WorkerStateMachine();
        CurrentJob = WorkerJobType.None;

        idleState = new WorkerIdleState(this);
        findResourceState = new WorkerFindResourceState(this);
        goToResourceState = new WorkerGoToResourceState(this);
        workState = new WorkerWorkState(this);
        carryState = new WorkerCarryState(this);

        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.NotEmptyCollection(this, config, nameof(config));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: WorkerConfig is invalid.", this);
            valid = false;
        }

        return valid;
    }

    private void Update()
    {
        if (!enabled || StateMachine == null)
            return;

        StateMachine.Update();
    }

    private void OnDestroy()
    {
        ClearCurrentAssignment();

        Home?.RemoveWorker(this);
        WorkerRegistry.Instance?.Unregister(this);
    }

    public void SetHome(House house)
    {
        Home = house;
        WorkerRegistry.Instance?.Register(this);
        EnterIdleState();
    }

    public void AssignJob(WorkerJobType job)
    {
        if (Brain == null)
            return;

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
        if (StateMachine == null)
            return;

        bool changed = StateMachine.ChangeState(state);
        if (changed)
            OnActivityChanged?.Invoke();
    }

    public void EnterIdleState()
    {
        ChangeState(idleState);
    }

    public void EnterFindResourceState()
    {
        ChangeState(findResourceState);
    }

    public void EnterGoToResourceState()
    {
        ChangeState(goToResourceState);
    }

    public void EnterWorkState()
    {
        ChangeState(workState);
    }

    public void EnterCarryState()
    {
        ChangeState(carryState);
    }

    public bool CanSwitchJobImmediately()
    {
        return TargetResource == null &&
               TargetSlot == null &&
               Inventory != null &&
               !Inventory.HasCargo &&
               Movement != null &&
               !Movement.HasTarget;
    }

    public void ClearCurrentAssignment()
    {
        if (TargetResource != null)
            TargetResource.CancelWork(this);

        TargetResource = null;
        TargetSlot = null;
    }

    public void ResetTaskState()
    {
        ClearCurrentAssignment();
        Inventory?.Clear();
        Animator?.SetWorking(false);
        OnActivityChanged?.Invoke();
    }

    public void DeliverCargo()
    {
        if (Inventory == null || !Inventory.HasCargo)
            return;

        if (ResourceDepositService.Instance == null)
            return;

        int amount = Inventory.TakeCargo(out ResourceType resourceType);
        ResourceDepositService.Instance.Deposit(resourceType, amount);
    }

    public void StartFindingResource()
    {
        EnterFindResourceState();
    }

    public void GoIdle()
    {
        EnterIdleState();
    }

    public bool HasValidResourceAssignmentForMove()
    {
        return WorkerResourceSelector.HasValidAssignmentForMove(this);
    }

    public bool HasValidResourceAssignmentForWork()
    {
        return WorkerResourceSelector.HasValidAssignmentForWork(this);
    }

    public float GetReachResourceDistance()
    {
        return config != null ? config.ReachResourceDistance : 0.3f;
    }

    public float GetMaxWorkDistance()
    {
        return config != null ? config.MaxWorkDistance : 0.35f;
    }
}


