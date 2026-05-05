using UnityEngine;

/// <summary>
/// Сервис поиска ресурсных точек
/// Использует ResourceRegistry как источник всех активных ресурсов на сцене
/// </summary>
public sealed class ResourceSearchService
{
    private const float PriorityWeight = 100f;

    private readonly ResourceRegistry _resourceRegistry;

    public ResourceSearchService(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    /// <summary>
    /// Ищет лучший доступный ресурс нужного типа рядом с указанной позицией
    /// </summary>
    public T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        if (_resourceRegistry == null)
            return null;

        T bestResource = null;
        float bestScore = float.MinValue;

        foreach (IResourceNode node in _resourceRegistry.Nodes)
        {
            if (node is not T resource)
                continue;

            if (!resource.IsAvailable)
                continue;

            if (!resource.HasFreeSlot())
                continue;

            float distanceScore = GetDistanceScore(resource, from);
            float priorityScore = resource.Priority * PriorityWeight;
            float finalScore = priorityScore - distanceScore;

            if (finalScore <= bestScore)
                continue;

            bestScore = finalScore;
            bestResource = resource;
        }

        return bestResource;
    }

    private static float GetDistanceScore(ResourceNodeBase resource, Vector2 from)
    {
        Vector2 workPosition = resource.GetWorkPosition(null);
        return (workPosition - from).sqrMagnitude;
    }
}