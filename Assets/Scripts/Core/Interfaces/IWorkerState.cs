/// <summary>
/// Контракт состояния worker'а.
/// Каждое состояние отвечает за отдельный этап поведения рабочего.
/// </summary>
public interface IWorkerState
{
    /// <summary>
    /// Вызывается при входе в состояние.
    /// </summary>
    void Enter();

    /// <summary>
    /// Вызывается каждый кадр, пока состояние активно.
    /// </summary>
    void Update();

    /// <summary>
    /// Вызывается при выходе из состояния.
    /// </summary>
    void Exit();
}
