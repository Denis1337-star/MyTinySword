using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourceFinder
{
    public static T FindBest<T>(Vector2 from) where T : ResourceNodeBase
    {
        T best = null;
        float bestScore = float.MinValue;

        foreach (var node in ResourceRegistry.Instance.Nodes)
        {
            if (node is not T typed)
                continue;

            var freeSlot = typed.GetFreeSlot(null); // проверяем доступность слота
            if (freeSlot == null)
                continue; // нет свободных слотов → пропускаем

            float dist = Vector2.Distance(from, typed.WorkPosition);
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
