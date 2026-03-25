using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourceFinder
{
    private const float PriorityWeight = 100f;

    public static T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        if (ResourceRegistry.Instance == null)
            return null;

        T best = null;
        float bestScore = float.MinValue;

        foreach (var node in ResourceRegistry.Instance.Nodes)
        {
            if (node is not T typed)
                continue;

            if (!typed.IsAvailable)
                continue;

            if (!typed.HasFreeSlot())
                continue;

            float dist = Vector2.Distance(from, typed.WorkPosition);
            float score = typed.Priority * PriorityWeight - dist;

            if (score > bestScore)
            {
                bestScore = score;
                best = typed;
            }
        }

        return best;
    }
}
