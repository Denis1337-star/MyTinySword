using UnityEngine;

public class WorkerStateMachine 
{
    private IWorkerState currentState;

    public IWorkerState CurrentState => currentState;
    public string CurrentStateName => currentState?.GetType().Name ?? "None";

    public void ChangeState(IWorkerState newState)
    {
        if (newState == null)
            return;

        if (currentState == newState)
            return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}
