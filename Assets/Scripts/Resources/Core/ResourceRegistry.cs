using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реестр всех  ресурсных точек на сцене
/// </summary>
public sealed class ResourceRegistry 
{
    private readonly List<ResourceNodeBase> _nodes = new();

    public IReadOnlyList<ResourceNodeBase> Nodes => _nodes;
    public int Count => _nodes.Count;

    public void Register(ResourceNodeBase node)
    {
        if (node == null)
            return;

        if (_nodes.Contains(node))
            return;

        _nodes.Add(node);
    }

    public void Unregister(ResourceNodeBase node)
    {
        if (node == null)
            return;

        _nodes.Remove(node);
    }

    /// <summary>
    /// Ищет ближайший доступный ресурс конкретного типа
    /// </summary>
    public T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        T bestResource = null;
        float bestSqrDistance = float.MaxValue;

        for (int i = 0; i < _nodes.Count; i++)
        {
            ResourceNodeBase node = _nodes[i];

            if (node == null)
                continue;

            if (node is not T resource)
                continue;

            if (!resource.IsAvailable)
                continue;

            if (!resource.HasFreeSlot())
                continue;

            float sqrDistance = GetSqrDistanceToResource(resource, from);

            if (sqrDistance >= bestSqrDistance)
                continue;

            bestSqrDistance = sqrDistance;
            bestResource = resource;
        }

        return bestResource;
    }

    private static float GetSqrDistanceToResource(ResourceNodeBase resource, Vector2 from)
    {
        Vector2 workPosition = resource.GetWorkPosition(null);
        return (workPosition - from).sqrMagnitude;
    }
}