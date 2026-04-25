using UnityEngine;

/// <summary>
/// Отвечает за поиск лучшего ресурса для worker
/// </summary>
public static class ResourceFinder
{
    private const float PriorityWeight = 100f;

    public static T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        if (ResourceRegistry.Instance == null)
            return null;

        T best = null;
        float bestScore = float.MinValue;

        foreach (IResourceNode node in ResourceRegistry.Instance.Nodes)
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
