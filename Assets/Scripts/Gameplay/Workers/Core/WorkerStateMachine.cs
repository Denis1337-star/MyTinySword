
/// <summary>
///машина состояний worker
/// </summary>
public class WorkerStateMachine 
{
    private IWorkerState currentState;

    public IWorkerState CurrentState => currentState;
    public string CurrentStateName => currentState?.GetType().Name ?? "None";

    /// <summary>
    /// Переключает в новое состояние
    /// </summary>
    public bool ChangeState(IWorkerState newState)
    {
        if (newState == null)
            return false;

        if (currentState == newState)
            return false;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();

        return true;
    }

    /// <summary>
    /// Обновляет текущее активное состояние
    /// </summary>
    public void Update()
    {
        currentState?.Update();
    }
}
