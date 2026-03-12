using System;
using System.Collections;
using UnityEngine;


//public enum WorkerState
//{
//    Idle,               // стоит, ждёт приказа
//    GoingToResource,    // идёт к ресурсу
//    Working,            // работает (рубит / добывает)
//    CarryingToHouse,     // несёт ресурсы домой
//    Unloading
//}
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
   public WorkerStateMachine StateMachine { get; private set; }
    public UnitMovement Movement { get; private set; }
    public WorkerAnimator Animator { get; private set; }
    public House Home { get; private set; }
    public IWorkerJob CurrentJobLogic { get; private set; }
    public WorkerJobType CurrentJob { get; private set; }

    public WorkerJobType PendingJob { get; private set; } = WorkerJobType.None;

    public ResourceNodeBase TargetResource { get; set; }
    public WorkSlot TargetSlot { get; set; }
    public int CarriedAmount { get; set; }

    public event Action OnJobChanged;
    public event Action OnActivityChanged;

    private void Awake()
    {
        Movement = GetComponent<UnitMovement>();
        Animator = GetComponent<WorkerAnimator>();
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
        if (Home == null)
            return;

        // Если сейчас ничего не делает и не несёт ресурс — можно сразу переключать
        bool canSwitchNow =
            TargetResource == null &&
            TargetSlot == null &&
            CarriedAmount == 0 &&
            !Movement.HasTarget;

        if (canSwitchNow || CurrentJob == WorkerJobType.None)
        {
            ApplyJobImmediately(job);
            return;
        }

        // Иначе ставим новую работу в очередь
        PendingJob = job;
    }

    public void ApplyPendingJobIfAny()
    {
        if (PendingJob == WorkerJobType.None)
            return;

        WorkerJobType nextJob = PendingJob;
        PendingJob = WorkerJobType.None;
        ApplyJobImmediately(nextJob);
    }

    private void ApplyJobImmediately(WorkerJobType job)
    {
        if (TargetResource != null)
            TargetResource.CancelWork(this);

        TargetResource = null;
        TargetSlot = null;
        CarriedAmount = 0;

        CurrentJob = job;
        CurrentJobLogic = WorkerJobFactory.Create(job);

        OnJobChanged?.Invoke();
        ChangeState(new WorkerFindResourceState(this));
    }

    public void ChangeState(IWorkerState state)
    {
        StateMachine.ChangeState(state);
        OnActivityChanged?.Invoke();
    }
}
