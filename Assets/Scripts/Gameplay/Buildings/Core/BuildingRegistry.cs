using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реестр зданий на сцене.
/// Хранит построенные и строящиеся здания.
/// </summary>
public sealed class BuildingRegistry 
{
    private readonly Dictionary<string, int> _builtBuildingCounts = new();
    private readonly Dictionary<string, int> _constructingBuildingCounts = new();
    private readonly Dictionary<string, BuildingBase> _builtBuildingsById = new();

    private readonly TechTreeBonusService _techTreeBonusService;

    public event Action<BuildingConfig> BuildingBuilt;

    public BuildingRegistry(TechTreeBonusService techTreeBonusService)
    {
        _techTreeBonusService = techTreeBonusService;
    }

    public bool IsBuiltOrConstructing(BuildingConfig config)
    {
        return GetBuiltOrConstructingCount(config) > 0;
    }

    public bool IsLimitReached(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return false;

        if (!config.UniqueBuilding)
            return false;

        return GetBuiltOrConstructingCount(config) >= GetAllowedCount(config);
    }

    public int GetBuiltOrConstructingCount(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return 0;

        string buildingId = config.BuildingId;

        return GetCount(_builtBuildingCounts, buildingId) +
               GetCount(_constructingBuildingCounts, buildingId);
    }

    public int GetAllowedCount(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return 0;

        if (!config.UniqueBuilding)
            return int.MaxValue;

        int bonusLimit = config.LimitBonusType != TechTreeBonusType.None
            ? _techTreeBonusService.GetBonusInt(config.LimitBonusType)
            : 0;

        return Mathf.Max(1, 1 + bonusLimit);
    }

    public void RegisterConstruction(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        AddCount(_constructingBuildingCounts, config.BuildingId);
    }

    public void UnregisterConstruction(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        RemoveCount(_constructingBuildingCounts, config.BuildingId);
    }

    public void RegisterBuilt(BuildingConfig config, BuildingBase building = null)
    {
        if (!IsValidConfig(config))
            return;

        RemoveCount(_constructingBuildingCounts, config.BuildingId);
        AddCount(_builtBuildingCounts, config.BuildingId);

        if (building != null && !_builtBuildingsById.ContainsKey(config.BuildingId))
            _builtBuildingsById[config.BuildingId] = building;

        BuildingBuilt?.Invoke(config);
    }

    public void UnregisterBuilt(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        RemoveCount(_builtBuildingCounts, config.BuildingId);

        if (GetCount(_builtBuildingCounts, config.BuildingId) <= 0)
            _builtBuildingsById.Remove(config.BuildingId);
    }

    public Transform FindBuiltBuildingTransform(BuildingConfig config)
    {
        if (!TryGetBuiltBuilding(config, out BuildingBase building))
            return null;

        return building.transform;
    }

    public bool TryGetBuiltBuilding(BuildingConfig config, out BuildingBase building)
    {
        building = null;

        if (!IsValidConfig(config))
            return false;

        return _builtBuildingsById.TryGetValue(config.BuildingId, out building) &&
               building != null;
    }

    private static int GetCount(
        Dictionary<string, int> counts,
        string buildingId)
    {
        if (counts == null || string.IsNullOrWhiteSpace(buildingId))
            return 0;

        return counts.TryGetValue(buildingId, out int count)
            ? count
            : 0;
    }

    private static void AddCount(
        Dictionary<string, int> counts,
        string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return;

        counts.TryGetValue(buildingId, out int count);
        counts[buildingId] = count + 1;
    }

    private static void RemoveCount(
        Dictionary<string, int> counts,
        string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return;

        if (!counts.TryGetValue(buildingId, out int count))
            return;

        count--;

        if (count <= 0)
        {
            counts.Remove(buildingId);
            return;
        }

        counts[buildingId] = count;
    }

    private static bool IsValidConfig(BuildingConfig config)
    {
        return config != null &&
               !string.IsNullOrWhiteSpace(config.BuildingId);
    }
}