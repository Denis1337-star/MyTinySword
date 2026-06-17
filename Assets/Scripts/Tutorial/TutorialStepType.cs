/// <summary>
/// “ип шага обучени€.
/// Message Ч обычный текстовый шаг.
/// ќстальные типы ждут конкретное действие игрока.
/// </summary>
public enum TutorialStepType
{
    Message = 0,
    SelectHouse = 1,
    AssignWorkersToWood = 2,
    BuildRequiredBuilding = 3,
    HireArmyUnit = 4,
    SelectArmy = 5,
    AttackEnemy = 6,
    WinLevel = 7
}