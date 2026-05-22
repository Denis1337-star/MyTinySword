using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Место где  можно  строить здания
/// </summary>
public sealed class ConstructionSlot : ValidatedMonoBehaviour
{
    [SerializeField] private List<BuildingConfig> _availableBuildings = new();
    [SerializeField] private ConstructionSite _constructionPrefab;

    private ConstructionSite _currentConstruction;
    private ResourceStorage _resourceStorage;
    private BuildingRegistry _buildingRegistry;
    private BuildingFactory _buildingFactory;

    public IReadOnlyList<BuildingConfig> AvailableBuildings => _availableBuildings;
    public bool HasConstruction => _currentConstruction != null;

    [Inject]
    private void Construct(
        ResourceStorage resourceStorage,
        BuildingRegistry buildingRegistry,
        BuildingFactory buildingFactory)
    {
        _resourceStorage = resourceStorage;
        _buildingRegistry = buildingRegistry;
        _buildingFactory = buildingFactory;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.NotEmptyList(this, _availableBuildings, nameof(_availableBuildings));
        valid &= ValidationUtility.IsAssigned(this, _constructionPrefab, nameof(_constructionPrefab));

        return valid;
    }

    public bool CanBuild(BuildingConfig config = null)
    {
        if (config == null)
            return _currentConstruction == null;

        return string.IsNullOrEmpty(GetBuildBlockReason(config));
    }

    public string GetBuildBlockReason(BuildingConfig config)
    {
        if (config == null)
            return "Здание не выбрано";

        if (_currentConstruction != null)
            return "Уже строится";

        if (config.UniqueBuilding && _buildingRegistry.IsBuiltOrConstructing(config))
            return "Лимит достигнут";

        if (!_resourceStorage.HasResources(config.WoodCost, config.GoldCost, 0))
            return "Не хватает ресурсов";

        return string.Empty;
    }
    public bool IsUniqueBuildingBlocked(BuildingConfig config)
    {
        if (config == null)
            return false;

        if (!config.UniqueBuilding)
            return false;

        return _buildingRegistry.IsBuiltOrConstructing(config);
    }

    public bool StartConstruction(BuildingConfig config)
    {
        if (config == null)
            return false;

        if (!config.IsValid())
            return false;

        if (!CanBuild(config))
            return false;

        ConstructionSite site = _buildingFactory.CreateConstructionSite(
            _constructionPrefab, transform.position,Quaternion.identity);

        if (site == null)
            return false;

        bool spent = _resourceStorage.TrySpendResources(
            config.WoodCost,config.GoldCost, 0);

        if (!spent)
        {
            Destroy(site.gameObject);
            return false;
        }

        _currentConstruction = site;

        _buildingRegistry.RegisterConstruction(config);

        site.Initialize(this, config, _buildingRegistry, _buildingFactory);

        gameObject.SetActive(false);

        return true;
    }

    public void OnConstructionFinished()
    {
        _currentConstruction = null;
    }

    public void Restore()
    {
        _currentConstruction = null;
        gameObject.SetActive(true);
    }
}