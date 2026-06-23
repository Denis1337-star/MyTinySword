/// <summary>
/// Тип бонуса, который даёт нода дерева развития.
/// Используется gameplay-кодом, чтобы не обращаться к бонусам через строки.
/// </summary>
public enum TechTreeBonusType
{
    None,

    StartWood,
    StartGold,
    StartMeat,

    StartWorkers,
    WorkersSpeed,
    WorkersYield,
    WorkersGather,

    DemolishRefund,

    BuildingHp,
    BuildAll,
    LimitBarrack,
    LimitArchery,
    LimitMonastery,

    ArmyCap,
    HireArmy,
    TrainArmy,
    StatsArmy,
    QueueMilitaryBuildings,

    TowerDamage,
    TowerRange,
    TowerFireRate
}