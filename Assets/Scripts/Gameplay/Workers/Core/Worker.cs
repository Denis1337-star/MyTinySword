using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Центральная сущность worker'а.
/// Хранит ссылки на компоненты, состояние работы, текущий ресурс и управляет worker state machine.
/// </summary>
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(WorkerInventory))]
[RequireComponent(typeof(WorkerBrain))]
public class Worker : ValidatedMonoBehaviour
{
    [Header("Config")]
    [SerializeField] private WorkerConfig _config;

    private WorkerIdleState _idleState;
    private WorkerFindResourceState _findResourceState;
    private WorkerGoToResourceState _goToResourceState;
    private WorkerWorkState _workState;
    private WorkerCarryState _carryState;

    private WorkerRegistry _workerRegistry;

    private ResourceStorage _resourceStorage;

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

    public WorkerConfig Config => _config;
    public ResourceStorage ResourceStorage => _resourceStorage;

    public event Action OnJobChanged;
    public event Action OnActivityChanged;

    public string CurrentStateName => StateMachine != null
        ? StateMachine.CurrentStateName
        : "None";

    public bool HasCargo => Inventory != null && Inventory.HasCargo;
    public bool HasPendingJob => PendingJob != WorkerJobType.None;


    [Inject]
    private void Construct( WorkerRegistry workerRegistry, ResourceStorage resourceStorage)
    {
        _workerRegistry = workerRegistry;
        _resourceStorage = resourceStorage;
    }

    protected override void Awake()
    {
        Movement = GetComponent<UnitMovement>();
        Animator = GetComponent<WorkerAnimator>();
        Inventory = GetComponent<WorkerInventory>();
        Brain = GetComponent<WorkerBrain>();

        StateMachine = new WorkerStateMachine();
        CurrentJob = WorkerJobType.None;

        _idleState = new WorkerIdleState(this);
        _findResourceState = new WorkerFindResourceState(this);
        _goToResourceState = new WorkerGoToResourceState(this);
        _workState = new WorkerWorkState(this);
        _carryState = new WorkerCarryState(this);

        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));

        if (_config != null && !_config.IsValid())
        {
            Debug.LogError($"{name}: WorkerConfig некорректный.", this);
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
        _workerRegistry?.Unregister(this);
    }

    public void SetHome(House house)
    {
        Home = house;

        _workerRegistry?.Register(this);

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
        ChangeState(_idleState);
    }

    public void EnterFindResourceState()
    {
        ChangeState(_findResourceState);
    }

    public void EnterGoToResourceState()
    {
        ChangeState(_goToResourceState);
    }

    public void EnterWorkState()
    {
        ChangeState(_workState);
    }

    public void EnterCarryState()
    {
        ChangeState(_carryState);
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


        int amount = Inventory.TakeCargo(out ResourceType resourceType);
        _resourceStorage.AddResource(resourceType, amount);
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
        return _config != null ? _config.ReachResourceDistance : 0.3f;
    }

    public float GetMaxWorkDistance()
    {
        return _config != null ? _config.MaxWorkDistance : 0.35f;
    }
}