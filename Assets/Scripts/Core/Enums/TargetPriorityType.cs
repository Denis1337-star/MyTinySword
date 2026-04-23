
/// <summary>
/// Приоритет типа цели для боевых юнитов.
/// Нужен, чтобы юниты сначала били врагов, потом башни, потом здания.
/// </summary>
public enum TargetPriorityType
{
    None = 0,
    ArmyUnit = 100,
    Tower = 80,
    Building = 50,
    Castle = 10
}
