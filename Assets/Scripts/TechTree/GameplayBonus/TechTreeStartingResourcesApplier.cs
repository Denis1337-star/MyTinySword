using UnityEngine;
using Zenject;

/// <summary>
/// Добавляет стартовые ресурсы из дерева развития при запуске gameplay-сцены.
/// </summary>
public sealed class TechTreeStartingResourcesApplier : MonoBehaviour
{
    private ResourceStorage _resourceStorage;
    private TechTreeBonusService _bonusService;

    private bool _applied;

    [Inject]
    private void Construct(
        ResourceStorage resourceStorage,
        TechTreeBonusService bonusService)
    {
        _resourceStorage = resourceStorage;
        _bonusService = bonusService;
    }

    private void Start()
    {
        ApplyBonuses();
    }

    private void ApplyBonuses()
    {
        if (_applied)
            return;

        _applied = true;

        AddBonusResource(ResourceType.Wood, TechTreeBonusType.StartWood);
        AddBonusResource(ResourceType.Gold, TechTreeBonusType.StartGold);
        AddBonusResource(ResourceType.Meat, TechTreeBonusType.StartMeat);
    }

    private void AddBonusResource(
        ResourceType resourceType,
        TechTreeBonusType bonusType)
    {
        int bonusAmount = _bonusService.GetBonusInt(bonusType);

        if (bonusAmount <= 0)
            return;

        _resourceStorage.AddResource(resourceType, bonusAmount);

        Debug.Log($"[TechTreeStartingResourcesApplier] Добавлен стартовый ресурс {resourceType}: +{bonusAmount}.");
    }
}