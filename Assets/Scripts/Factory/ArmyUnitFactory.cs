using UnityEngine;
using Zenject;

/// <summary>
/// Фабрика для создания боевых юнитов через Zenject
/// </summary>
public sealed class ArmyUnitFactory
{
    private readonly DiContainer _container;

    public ArmyUnitFactory(DiContainer container)
    {
        _container = container;
    }

    public GameObject Create(GameObject prefab,
        Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("ArmyUnitFactory: prefab юнита не назначен.");
            return null;
        }

        return _container.InstantiatePrefab(
            prefab, position, rotation, null);
    }
}