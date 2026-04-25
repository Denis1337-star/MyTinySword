
/// <summary>
/// Контракт состояния worker
/// </summary>
public interface IWorkerState
{
    void Enter();  
    void Update(); 
    void Exit(); 
}
