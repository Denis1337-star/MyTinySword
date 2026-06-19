using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реестр зданий на сцене.
/// Хранит построенные и строящиеся здания.
/// </summary>
public sealed class BuildingRegistry : MonoBehaviour
{
    private readonly HashSet<string> _builtBuildingIds = new();
    private readonly HashSet<string> _constructingBuildingIds = new();
    private readonly Dictionary<string, BuildingBase> _builtBuildingsById = new();

    public event Action<BuildingConfig> BuildingBuilt;

    public bool IsBuiltOrConstructing(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return false;

        return _builtBuildingIds.Contains(config.BuildingId) ||
               _constructingBuildingIds.Contains(config.BuildingId);
    }

    public void RegisterConstruction(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        _constructingBuildingIds.Add(config.BuildingId);
    }

    public void UnregisterConstruction(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        _constructingBuildingIds.Remove(config.BuildingId);
    }

    public void RegisterBuilt(BuildingConfig config, BuildingBase building = null)
    {
        if (!IsValidConfig(config))
            return;

        _constructingBuildingIds.Remove(config.BuildingId);
        _builtBuildingIds.Add(config.BuildingId);

        if (building != null)
            _builtBuildingsById[config.BuildingId] = building;

        BuildingBuilt?.Invoke(config);
    }

    public void UnregisterBuilt(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        _builtBuildingIds.Remove(config.BuildingId);
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

    private static bool IsValidConfig(BuildingConfig config)
    {
        return config != null &&
               !string.IsNullOrWhiteSpace(config.BuildingId);
    }

    private void OnDestroy()
    {
        _builtBuildingIds.Clear();
        _constructingBuildingIds.Clear();
        _builtBuildingsById.Clear();
        BuildingBuilt = null;
    }
}
