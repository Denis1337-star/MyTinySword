/// <summary>
/// Сервис расчёта бонусов дерева развития для gameplay-систем.
/// </summary>
public sealed class TechTreeBonusService
{
    private readonly TechTreeSaveService _saveService;
    private readonly TechTreeCatalogConfig _catalog;

    public TechTreeBonusService(
        TechTreeSaveService saveService,
        TechTreeCatalogConfig catalog)
    {
        _saveService = saveService;
        _catalog = catalog;
    }

    public float GetBonusValue(TechTreeBonusType bonusType)
    {
        if (bonusType == TechTreeBonusType.None)
            return 0f;

        if (_catalog == null || _catalog.Nodes == null)
            return 0f;

        float totalBonus = 0f;

        for (int i = 0; i < _catalog.Nodes.Count; i++)
        {
            TechTreeNodeConfig config = _catalog.Nodes[i];

            if (config == null)
                continue;

            if (config.BonusType != bonusType)
                continue;

            TechTreeNodeSaveData saveData = _saveService.GetOrCreateNode(config);

            totalBonus += config.BonusPerLevel * saveData.Level;
        }

        return totalBonus;
    }

    public int GetBonusInt(TechTreeBonusType bonusType)
    {
        return (int)GetBonusValue(bonusType);
    }

    public float ApplyPercentBonus(float baseValue, TechTreeBonusType bonusType)
    {
        float bonusPercent = GetBonusValue(bonusType);

        return baseValue * (1f + bonusPercent / 100f);
    }

    public float ApplyPercentReduction(float baseValue, TechTreeBonusType bonusType)
    {
        float reductionPercent = GetBonusValue(bonusType);
        float multiplier = 1f - reductionPercent / 100f;

        if (multiplier < 0f)
            multiplier = 0f;

        return baseValue * multiplier;
    }
}