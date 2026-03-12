using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourceFinder
{
    public static T FindBest<T>(Vector2 from) where T : class, IResourceNode
    {
        T best = null;
        float bestScore = float.MinValue;

        foreach (var node in ResourceRegistry.Instance.Nodes)
        {
            if (node is not T typed || !node.IsAvailable)
                continue;

            float dist = Vector2.Distance(from, node.WorkPosition);
            float score = typed.Priority * 100f - dist;

            if (score > bestScore)
            {
                bestScore = score;
                best = typed;
            }
        }

        return best;
    }
}
