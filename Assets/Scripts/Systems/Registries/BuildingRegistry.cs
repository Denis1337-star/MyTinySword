using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// Реестр зданий на сцене
/// </summary>
public class BuildingRegistry : MonoBehaviour
{
    private readonly HashSet<string> _builtBuildingIds = new();
    private readonly HashSet<string> _constructingBuildingIds = new();

    private readonly Subject<BuildingConfig> _constructionRegistered = new();
    private readonly Subject<BuildingConfig> _constructionUnregistered = new();
    private readonly Subject<BuildingConfig> _buildingBuilt = new();
    private readonly Subject<BuildingConfig> _buildingUnregistered = new();

    public IObservable<BuildingConfig> ConstructionRegistered => _constructionRegistered;
    public IObservable<BuildingConfig> ConstructionUnregistered => _constructionUnregistered;
    public IObservable<BuildingConfig> BuildingBuilt => _buildingBuilt;
    public IObservable<BuildingConfig> BuildingUnregistered => _buildingUnregistered;

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

        if (!_constructingBuildingIds.Add(config.BuildingId))
            return;

        _constructionRegistered.OnNext(config);
    }

    public void UnregisterConstruction(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        if (!_constructingBuildingIds.Remove(config.BuildingId))
            return;

        _constructionUnregistered.OnNext(config);
    }

    public void RegisterBuilt(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        _constructingBuildingIds.Remove(config.BuildingId);

        if (!_builtBuildingIds.Add(config.BuildingId))
            return;

        _buildingBuilt.OnNext(config);
    }

    public void UnregisterBuilt(BuildingConfig config)
    {
        if (!IsValidConfig(config))
            return;

        if (!_builtBuildingIds.Remove(config.BuildingId))
            return;

        _buildingUnregistered.OnNext(config);
    }

    private static bool IsValidConfig(BuildingConfig config)
    {
        return config != null &&
               !string.IsNullOrWhiteSpace(config.BuildingId);
    }

    private void OnDestroy()
    {
        _constructionRegistered.OnCompleted();
        _constructionUnregistered.OnCompleted();
        _buildingBuilt.OnCompleted();
        _buildingUnregistered.OnCompleted();

        _constructionRegistered.Dispose();
        _constructionUnregistered.Dispose();
        _buildingBuilt.Dispose();
        _buildingUnregistered.Dispose();

        _builtBuildingIds.Clear();
        _constructingBuildingIds.Clear();
    }
}
