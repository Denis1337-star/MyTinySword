using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Хранит ссылки на обязательные компоненты
/// </summary>
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(WorkerBrain))]
[RequireComponent(typeof(WorkerAnimator))]
public sealed class Worker : ValidatedMonoBehaviour
{
    [SerializeField] private WorkerConfig _config;

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

    public event Action OnJobChanged;
    public event Action OnActivityChanged;

    public string CurrentStateName => StateMachine.CurrentStateName;
    public bool HasCargo => Inventory.HasCargo;
    public bool HasPendingJob => PendingJob != WorkerJobType.None;

    [Inject]
    private void Construct(
        WorkerRegistry workerRegistry,
        ResourceStorage resourceStorage)
    {
        _workerRegistry = workerRegistry;
        _resourceStorage = resourceStorage;
    }

    protected override void Awake()
    {
        Movement = GetComponent<UnitMovement>();
        Animator = GetComponent<WorkerAnimator>();
        Inventory = new WorkerInventory();
        Brain = GetComponent<WorkerBrain>();

        StateMachine = new WorkerStateMachine(this);
        StateMachine.StateChanged += HandleStateChanged;

        CurrentJob = WorkerJobType.None;

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
        StateMachine.Update();
    }

    private void OnDestroy()
    {
        ClearCurrentAssignment();

        StateMachine.StateChanged -= HandleStateChanged;

        Home?.RemoveWorker(this);
        _workerRegistry.Unregister(this);
    }

    public void SetHome(House house)
    {
        Home = house;

        _workerRegistry.Register(this);

        StateMachine.ChangeState(WorkerStateType.Idle);
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

    public bool CanSwitchJobImmediately()
    {
        return TargetResource == null &&
               TargetSlot == null &&
               !Inventory.HasCargo &&
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

        Inventory.Clear();
        Animator.SetWorking(false);

        OnActivityChanged?.Invoke();
    }

    public void DeliverCargo()
    {
        if (!Inventory.HasCargo)
            return;

        int amount = Inventory.TakeCargo(out ResourceType resourceType);
        _resourceStorage.AddResource(resourceType, amount);
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
        return _config.ReachResourceDistance;
    }

    public float GetMaxWorkDistance()
    {
        return _config.MaxWorkDistance;
    }

    private void HandleStateChanged()
    {
        OnActivityChanged?.Invoke();
    }
}