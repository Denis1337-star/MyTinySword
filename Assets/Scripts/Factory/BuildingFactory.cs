using UnityEngine;
using Zenject;

/// <summary>
/// Фабрика для создания объектов строительства и готовых зданий через Zenject
/// </summary>
public sealed class BuildingFactory
{
    private readonly DiContainer _container;

    public BuildingFactory(DiContainer container)
    {
        _container = container;
    }

    public ConstructionSite CreateConstructionSite(
        ConstructionSite prefab,
        Vector3 position,
        Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("BuildingFactory: prefab ConstructionSite не назначен.");
            return null;
        }

        return _container.InstantiatePrefabForComponent<ConstructionSite>(
            prefab,
            position,
            rotation,
            null);
    }

    public GameObject CreateBuilding(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("BuildingFactory: prefab здания не назначен.");
            return null;
        }

        return _container.InstantiatePrefab(
            prefab,
            position,
            rotation,
            null);
    }
}
