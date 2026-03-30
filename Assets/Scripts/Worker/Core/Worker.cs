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

    public string CurrentStateName => StateMachine.CurrentStateName;
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

        valid &= ValidationUtility.Required(this, config, nameof(config));
        valid &= ValidationUtility.Required(this, Movement, nameof(Movement));
        valid &= ValidationUtility.Required(this, Inventory, nameof(Inventory));
        valid &= ValidationUtility.Required(this, Brain, nameof(Brain));

        if (config != null && !config.IsValid())
        {
            Debug.LogError($"{name}: WorkerConfig is invalid", this);
            valid = false;
        }

        return valid;
    }

    private void Update()
    {
        if (!enabled)
            return;

        StateMachine.Update();
    }

    private void OnDestroy()
    {
        ClearCurrentAssignment();

        Home?.RemoveWorker(this);
        WorkerRegistry.Instance?.Unregister(this);
    }

    /// <summary>
    /// Привязывает worker'а к дому и запускает его базовое idle-состояние
    /// </summary>
    public void SetHome(House house)
    {
        Home = house;
        WorkerRegistry.Instance?.Register(this);
        EnterIdleState();
    }

    /// <summary>
    /// Публичный вход для назначения новой профессии worker'у
    /// </summary>
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

    /// <summary>
    /// Переключает worker'а в новое состояние и уведомляет подписчиков об изменении активности
    /// </summary>
    public void ChangeState(IWorkerState state)
    {
        StateMachine.ChangeState(state);
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

    /// <summary>
    /// Проверяет, можно ли безопасно сразу сменить профессию
    /// </summary>
    public bool CanSwitchJobImmediately()
    {
        return TargetResource == null &&
               TargetSlot == null &&
               !Inventory.HasCargo &&
               !Movement.HasTarget;
    }

    /// <summary>
    /// Сбрасывает текущую привязку к ресурсу и освобождает рабочий слот
    /// </summary>

    public void ClearCurrentAssignment()
    {
        if (TargetResource != null)
        {
            TargetResource.CancelWork(this);
        }

        TargetResource = null;
        TargetSlot = null;
    }

    /// <summary>
    /// Полностью сбрасывает текущий task-state worker'а
    /// </summary>
    public void ResetTaskState()
    {
        ClearCurrentAssignment();
        Inventory.Clear();
        Animator?.SetWorking(false);
        OnActivityChanged?.Invoke();
    }

    /// <summary>
    /// Сдаёт переносимый ресурс в общую систему хранения
    /// </summary>
    public void DeliverCargo()
    {
        if (CurrentJobLogic == null || !Inventory.HasCargo || ResourceDepositService.Instance == null)
            return;

        int amount = Inventory.TakeCargo();
        ResourceDepositService.Instance.Deposit(CurrentJobLogic.RewardType, amount);
    }

    /// <summary>
    /// Переводит worker'а в состояние поиска нового ресурса
    /// </summary>
    public void StartFindingResource()
    {
        EnterFindResourceState();
    }

    /// <summary>
    /// Переводит worker'а в idle-состояние
    /// </summary>
    public void GoIdle()
    {
        EnterIdleState();
    }

    public bool HasValidResourceAssignment()
    {
        return WorkerResourceSelector.HasValidAssignment(this);
    }

    public float GetReachResourceDistance()
    {
        return config != null ? config.reachResourceDistance : 0.3f;
    }

    public float GetMaxWorkDistance()
    {
        return config != null ? config.maxWorkDistance : 0.35f;
    }
}

public static class WorkerJobLocalization
{
    public static string GetName(WorkerJobType job)
    {
        return job switch
        {
            WorkerJobType.None => "Без работы",
            WorkerJobType.ChopWood => "Дровосек",
            WorkerJobType.MineGold => "Шахтёр",
            WorkerJobType.HuntMeat => "Охотник",
            _ => "Неизвестно"
        };
    }
}
