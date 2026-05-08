/// <summary>
/// Тип состояния worker
/// </summary>
public enum WorkerStateType
{
    Idle = 0,
    FindResource = 1,
    GoToResource = 2,
    Work = 3,
    Carry = 4
}