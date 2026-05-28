/// <summary>
/// Идентификаторы игровых звуков
/// </summary>
public enum SoundId
{
    None = 0,

    // UI
    ButtonClick = 10,
    ToggleClick = 11,
    SliderChanged = 12,
    PanelOpen = 13,
    BuyUnit,

    // Buildings
    BuildingBuilt = 100,
    BuildingDemolished = 101,
    BuildingDamage = 102,

    // Units / Combat
    UnitDamaged = 200,
    UnitDied = 201,
    ArrowShoot = 202,
    MeleeHit,
    Heal = 204,

    // Worker / Resources
    TreeChop = 300,
    GoldMine = 301,
    HuntMeat = 302,
    WorkerStep = 303,
    MoveCommand = 304
}