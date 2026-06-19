using UnityEngine;
using Zenject;

/// <summary>
/// Фабрика для создания worker через Zenject
/// </summary>
public sealed class WorkerFactory
{
    private readonly DiContainer _container;

    public WorkerFactory(DiContainer container)
    {
        _container = container;
    }

    public Worker Create(Worker prefab,
        Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("WorkerFactory: prefab worker не назначен.");
            return null;
        }

        return _container.InstantiatePrefabForComponent<Worker>(
            prefab, position, rotation, null);
    }
}