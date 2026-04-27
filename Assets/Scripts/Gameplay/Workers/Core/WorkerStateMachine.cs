
/// <summary>
///машина состояний worker
/// </summary>
public class WorkerStateMachine 
{
    private IWorkerState _currentState;

    public string CurrentStateName => _currentState != null
        ? _currentState.GetType().Name
        : "None";

    public bool ChangeState(IWorkerState newState)
    {
        if (newState == null)
            return false;

        if (_currentState == newState)
            return false;

        _currentState?.Exit();

        _currentState = newState;

        _currentState.Enter();

        return true;
    }

    public void Update()
    {
        _currentState?.Update();
    }
}
