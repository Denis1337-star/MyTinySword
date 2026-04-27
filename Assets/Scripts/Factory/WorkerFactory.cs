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

    public Worker Create(
        Worker prefab,
        Vector3 position,
        Quaternion rotation)
    {

        return _container.InstantiatePrefabForComponent<Worker>(
            prefab,
            position,
            rotation,
            null);
    }
}
