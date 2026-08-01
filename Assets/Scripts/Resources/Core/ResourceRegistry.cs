using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    /// Ищет ближайший доступный ресурс по стоимости пути NavMesh (с Area Cost).
    /// </summary>
    public T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        T bestByPath = null;
        float bestPathCost = float.MaxValue;

        T bestByFallback = null;
        float bestSqrFallback = float.MaxValue;

        Vector3 fromPosition = new(from.x, from.y, 0f);
        int areaMask = NavMesh.AllAreas;

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

            Vector2 workPosition = resource.GetWorkPosition(null);
            Vector3 toPosition = new(workPosition.x, workPosition.y, 0f);

            if (NavMeshPathCostEvaluator.TryGetPathCost(fromPosition, toPosition, areaMask, out float pathCost))
            {
                if (pathCost >= bestPathCost)
                    continue;

                bestPathCost = pathCost;
                bestByPath = resource;
                continue;
            }

            float sqrDistance = (workPosition - from).sqrMagnitude;
            if (sqrDistance >= bestSqrFallback)
                continue;

            bestSqrFallback = sqrDistance;
            bestByFallback = resource;
        }

        return bestByPath != null ? bestByPath : bestByFallback;
    }
}
