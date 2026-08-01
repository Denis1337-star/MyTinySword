using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Хранит ссылки на обязательные компоненты
/// </summary>
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(WorkerAnimator))]
public sealed class Worker : ValidatedMonoBehaviour
{
    [SerializeField] private WorkerConfig _config;
    [SerializeField] private UnitMovement _movement;
    [SerializeField] private WorkerAnimator _animator;
    [SerializeField] private Health _health;
    [SerializeField] private FactionType _faction = FactionType.Player;

    private WorkerBrain _brain;
    private WorkerRegistry _workerRegistry;
    private ResourceStorage _resourceStorage;
    private TechTreeBonusService _techTreeBonusService;

    public WorkerStateMachine StateMachine { get; private set; }
    public UnitMovement Movement => _movement;
    public WorkerAnimator Animator => _animator;
    public Health Health => _health;
    public FactionType Faction => _faction;
    public WorkerInventory Inventory { get; private set; }
    public WorkerBrain Brain => _brain;

    public House Home { get; private set; }

    /// <summary>
    /// Порядковый номер рабочего для UI (1, 2, …). Не путать с GameObject.name.
    /// </summary>
    public int DisplayNumber { get; private set; }

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

    private void OnEnable() => _health.OnDied += HandleDeath;
    private void OnDisable() => _health.OnDied -= HandleDeath;

    [Inject]
    private void Construct(
        WorkerRegistry workerRegistry,
        ResourceStorage resourceStorage,
        TechTreeBonusService techTreeBonusService,
        ResourceRegistry resourceRegistry)   
    {
        _workerRegistry = workerRegistry;
        _resourceStorage = resourceStorage;
        _techTreeBonusService = techTreeBonusService;
        _brain = new WorkerBrain(this, resourceRegistry);   
    }

    protected override void Awake()
    {
        Inventory = new WorkerInventory();

        StateMachine = new WorkerStateMachine(this);
        StateMachine.StateChanged += HandleStateChanged;

        CurrentJob = WorkerJobType.None;

        base.Awake();
        _health.Initialize(_config.MaxHealth);
    }
    private void Start()
    {
        ApplyMovementSpeedBonus();
        // Рабочие не объезжают агентов — проходят сквозь других рабочих и воинов.
        _movement.SetAvoidOtherAgents(false);
        _movement.SetAvoidancePriority(0);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsValidConfig(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _movement, nameof(_movement));
        valid &= ValidationUtility.IsAssigned(this, _animator, nameof(_animator));
        valid &= ValidationUtility.IsAssigned(this, _health, nameof(_health));

        return valid;
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void HandleDeath()
    {
        Destroy(gameObject);  
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

    public void SetDisplayNumber(int displayNumber)
    {
        DisplayNumber = displayNumber;
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
        if (Inventory.HasCargo)
            return false;

        if (TargetResource == null &&
            TargetSlot == null &&
            !Movement.HasTarget)
            return true;

        // Залип на NavMesh (путь есть, но не едет) — можно сменить работу сразу.
        if (Movement.HasFailedPath)
            return true;

        return Movement.HasTarget && !Movement.IsMoving;
    }

    public void ClearCurrentAssignment()
    {
        if (TargetResource != null)
            TargetResource.CancelWork(this);

        TargetResource = null;
        TargetSlot = null;
    }

    public void CompleteCurrentAssignment()
    {
        if (TargetResource != null)
            TargetResource.CompleteWork(this);

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
    public int ApplyYieldBonus(int baseAmount)
    {
        if (baseAmount <= 0)
            return 0;

        int bonusAmount = _techTreeBonusService.GetBonusInt(TechTreeBonusType.WorkersYield);

        return baseAmount + bonusAmount;
    }
    private void ApplyMovementSpeedBonus()
    {
        float speed = _techTreeBonusService.ApplyPercentBonus(
            _movement.Speed,
            TechTreeBonusType.WorkersSpeed);

        _movement.SetSpeed(speed);
    }

    public void ResetToIdle()
    {
        ClearCurrentAssignment();
        StateMachine.ChangeState(WorkerStateType.Idle);
    }

    public bool HasValidResourceAssignment()
    {
        return WorkerResourceSelector.HasValidAssignment(this);
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