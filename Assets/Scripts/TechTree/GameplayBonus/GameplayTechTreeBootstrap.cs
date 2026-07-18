using Zenject;

/// <summary>
/// Применяет стартовые бонусы tech tree на gameplay-сцене.
/// </summary>
public sealed class GameplayTechTreeBootstrap : IInitializable
{
    private readonly ResourceStorage _resourceStorage;
    private readonly TechTreeBonusService _bonusService;
    private readonly House _playerHouse;

    public GameplayTechTreeBootstrap(
        ResourceStorage resourceStorage,
        TechTreeBonusService bonusService,
        House playerHouse)
    {
        _resourceStorage = resourceStorage;
        _bonusService = bonusService;
        _playerHouse = playerHouse;
    }

    public void Initialize()
    {
        ApplyStartingResources();
        ApplyStartingWorkers();
    }

    private void ApplyStartingResources()
    {
        AddBonusResource(ResourceType.Wood, TechTreeBonusType.StartWood);
        AddBonusResource(ResourceType.Gold, TechTreeBonusType.StartGold);
        AddBonusResource(ResourceType.Meat, TechTreeBonusType.StartMeat);
    }

    private void AddBonusResource(ResourceType resourceType, TechTreeBonusType bonusType)
    {
        int bonusAmount = _bonusService.GetBonusInt(bonusType);

        if (bonusAmount <= 0)
            return;

        _resourceStorage.AddResource(resourceType, bonusAmount);
    }

    private void ApplyStartingWorkers()
    {
        int bonusWorkers = _bonusService.GetBonusInt(TechTreeBonusType.StartWorkers);

        if (bonusWorkers <= 0)
            return;

        _playerHouse.AddFreeWorkers(bonusWorkers);
    }
}
