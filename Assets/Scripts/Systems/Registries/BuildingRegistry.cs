using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реестр зданий на сцене
/// </summary>
public sealed class BuildingRegistry : MonoBehaviour
{
    private readonly HashSet<string> _builtBuildingIds = new();
    private readonly HashSet<string> _constructingBuildingIds = new();

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

    public void RegisterBuilt(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        _constructingBuildingIds.Remove(config.BuildingId);
        _builtBuildingIds.Add(config.BuildingId);
    }

    public void UnregisterBuilt(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        _builtBuildingIds.Remove(config.BuildingId);
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
    }
}