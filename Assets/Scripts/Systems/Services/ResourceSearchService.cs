using UnityEngine;

/// <summary>
/// »щет наилучший доступный узел ресурсов 
/// </summary>
public sealed class ResourceSearchService
{
    private const float PriorityWeight = 100f;

    private readonly ResourceRegistry _resourceRegistry;

    public ResourceSearchService(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    public T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        if (_resourceRegistry == null)
            return null;

        T best = null;
        float bestScore = float.MinValue;

        foreach (IResourceNode node in _resourceRegistry.Nodes)
        {
            if (node is not T typed)
                continue;

            if (!typed.IsAvailable)
                continue;

            if (!typed.HasFreeSlot())
                continue;

            float sqrDistance = (typed.GetWorkPosition(null) - from).sqrMagnitude;
            float score = typed.Priority * PriorityWeight - sqrDistance;

            if (score <= bestScore)
                continue;

            bestScore = score;
            best = typed;
        }

        return best;
    }
}