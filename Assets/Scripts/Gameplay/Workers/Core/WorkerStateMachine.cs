
/// <summary>
/// State machine worker'а.
/// ’ранит текущее состо€ние и отвечает за корректный переход между состо€ни€ми.
/// </summary>
public class WorkerStateMachine 
{
    private IWorkerState _currentState;

    public string CurrentStateName => _currentState != null
        ? _currentState.GetType().Name
        : "None";

    /// <summary>
    /// ћен€ет текущее состо€ние worker'а.
    /// ¬озвращает true, если состо€ние действительно изменилось.
    /// </summary>
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

    /// <summary>
    /// ќбновл€ет текущее состо€ние.
    /// </summary>
    public void Update()
    {
        _currentState?.Update();
    }
}
