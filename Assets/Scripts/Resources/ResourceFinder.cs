using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourceFinder
{
    public static T FindBest<T>(Vector2 from) where T : MonoBehaviour, IResourceNode
    {
        T[] nodes = Object.FindObjectsOfType<T>();

        T best = null;
        float bestScore = float.MinValue;

        foreach (var node in nodes)
        {
            if (!node.IsAvailable)
                continue;

            float distance = Vector2.Distance(from, node.WorkPosition);
            float score = node.Priority * 100f - distance;

            if (score > bestScore)
            {
                bestScore = score;
                best = node;
            }
        }

        return best;
    }
}
