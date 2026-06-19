using System;

/// <summary>
/// State machine worker
/// </summary>
public sealed class WorkerStateMachine
{
    private readonly WorkerIdleState _idleState;
    private readonly WorkerFindResourceState _findResourceState;
    private readonly WorkerGoToResourceState _goToResourceState;
    private readonly WorkerWorkState _workState;
    private readonly WorkerCarryState _carryState;

    private IWorkerState _currentState;

    public event Action StateChanged;

    public string CurrentStateName => _currentState != null
        ? _currentState.GetType().Name
        : "None";

    public WorkerStateMachine(Worker worker)
    {
        _idleState = new WorkerIdleState(worker);
        _findResourceState = new WorkerFindResourceState(worker);
        _goToResourceState = new WorkerGoToResourceState(worker);
        _workState = new WorkerWorkState(worker);
        _carryState = new WorkerCarryState(worker);
    }

    /// <summary>
    /// Меняет текущее состояние worker
    /// </summary>
    public bool ChangeState(WorkerStateType stateType)
    {
        IWorkerState newState = GetState(stateType);

        if (newState == null)
            return false;

        if (_currentState == newState)
            return false;

        _currentState?.Exit();

        _currentState = newState;
        _currentState.Enter();

        StateChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Обновляет текущее состояние
    /// </summary>
    public void Update()
    {
        _currentState?.Update();
    }

    private IWorkerState GetState(WorkerStateType stateType)
    {
        return stateType switch
        {
            WorkerStateType.Idle => _idleState,
            WorkerStateType.FindResource => _findResourceState,
            WorkerStateType.GoToResource => _goToResourceState,
            WorkerStateType.Work => _workState,
            WorkerStateType.Carry => _carryState,
            _ => null
        };
    }
}