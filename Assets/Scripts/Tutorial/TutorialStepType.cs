/// <summary>
/// Тип шага обучения. Порядок значений нельзя менять — они сериализуются в сцене.
/// </summary>
public enum TutorialStepType
{
    /// <summary>Полноэкранное сообщение.</summary>
    Message = 0,

    /// <summary>Выбрать дом на карте.</summary>
    SelectHouse = 1,

    /// <summary>Назначить всех рабочих на дрова.</summary>
    AssignWorkersToWood = 2,

    /// <summary>Выбрать слот для постройки.</summary>
    SelectConstructionSlot = 3,

    /// <summary>Нанять юнита в казарме.</summary>
    HireArmyUnit = 4,

    /// <summary>Выбрать армию на карте.</summary>
    SelectArmy = 5,

    /// <summary>Атаковать врага.</summary>
    AttackEnemy = 6,

    /// <summary>Дождаться победы (устаревший шаг, для совместимости).</summary>
    WinLevel = 7,

    /// <summary>Выбрать казарму в панели постройки.</summary>
    BuildBarrackInPanel = 8,

    /// <summary>Дождаться окончания строительства.</summary>
    WaitBuildingConstructed = 9,

    /// <summary>Выбрать построенную казарму.</summary>
    SelectBuiltBarrack = 10,

    /// <summary>Дождаться появления воина.</summary>
    WaitWarriorSpawn = 11,

    /// <summary>Сфокусировать камеру на враге (устаревший, объединён с AttackEnemy).</summary>
    FocusEnemy = 12,

    /// <summary>Камера следует за боем до конца.</summary>
    WaitBattleReach = 13,

    /// <summary>Финальное мотивационное сообщение.</summary>
    FinalMotivation = 14
}
