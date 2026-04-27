using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

/// <summary>
/// место  на котором можно построить здание
/// </summary>
public class ConstructionSlot : MonoBehaviour
{
    [Header("Available Buildings")]
    [FormerlySerializedAs("availableBuildings")]
    [SerializeField] private List<BuildingConfig> _availableBuildings = new();

    [Header("Construction")]
    [FormerlySerializedAs("constructionPrefab")]
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

    public bool CanBuild(BuildingConfig config = null)
    {
        if (config == null)
        {
            if (_currentConstruction != null)
                return false;

            if (_constructionPrefab == null)
                return false;

            return true;
        }

        return string.IsNullOrEmpty(GetBuildBlockReason(config));
    }

    public string GetBuildBlockReason(BuildingConfig config)
    {
        if (config == null)
            return "Здание не выбрано";

        if (_currentConstruction != null)
            return "Уже строится";

        if (_constructionPrefab == null)
            return "Prefab стройки не назначен";

        if (!_availableBuildings.Contains(config))
            return "Это здание нельзя построить здесь";

        if (config.UniqueBuilding &&
            _buildingRegistry != null &&
            _buildingRegistry.IsBuiltOrConstructing(config))
        {
            return "Лимит достигнут";
        }

        if (_resourceStorage == null)
            return "Хранилище ресурсов не найдено";

        if (!_resourceStorage.HasResources(config.WoodCost, config.GoldCost))
            return "Не хватает ресурсов";

        return string.Empty;
    }

    public bool StartConstruction(BuildingConfig config)
    {
        if (config == null)
            return false;

        if (!config.IsValid())
            return false;

        if (!CanBuild(config))
            return false;

        if (_resourceStorage == null)
            return false;

        if (_buildingFactory == null)
        {
            Debug.LogError($"{name}: BuildingFactory не внедрён через Zenject.", this);
            return false;
        }

        bool spent = _resourceStorage.TrySpendResources(config.WoodCost, config.GoldCost);
        if (!spent)
            return false;

        ConstructionSite site = _buildingFactory.CreateConstructionSite(
            _constructionPrefab,
            transform.position,
            Quaternion.identity);

        if (site == null)
            return false;

        _currentConstruction = site;

        _buildingRegistry?.RegisterConstruction(config);

        site.Initialize(this, config, _buildingRegistry, _buildingFactory);

        gameObject.SetActive(false);

        return true;
    }

    public void OnConstructionFinished()
    {
        _currentConstruction = null;
        Destroy(gameObject);
    }
}