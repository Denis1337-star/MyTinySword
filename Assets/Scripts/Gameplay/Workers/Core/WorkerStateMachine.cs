using UnityEngine;

/// <summary>
/// Простая конечная машина состояний worker'а
/// Хранит текущее состояние и управляет переходами между состояниями
/// </summary>
public class WorkerStateMachine 
{
    private IWorkerState currentState;

    public IWorkerState CurrentState => currentState;  //Текущее активное состояние
    public string CurrentStateName => currentState?.GetType().Name ?? "None";  //Имя текущего состояние

    /// <summary>
    /// Переключает машину в новое состояние
    /// Сначала вызывает Exit у старого состояния, затем Enter у нового
    /// </summary>
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
    /// <summary>
    /// Обновляет текущее состояние
    /// </summary>
    public void Update()
    {
        currentState?.Update();
    }
}
