using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerStateMachine 
{
    private IWorkerState currentState;

    public void ChangeState(IWorkerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}