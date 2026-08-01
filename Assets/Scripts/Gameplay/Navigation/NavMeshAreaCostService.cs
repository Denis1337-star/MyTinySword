using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Глобальные стоимости NavMesh Area.
/// Влияет на выбор пути агентами и на поиск ресурса рабочими.
/// </summary>
public static class NavMeshAreaCostService
{
    private static readonly float[] AreaCosts = CreateDefaultCosts();

    private static float[] CreateDefaultCosts()
    {
        float[] costs = new float[32];
        for (int i = 0; i < costs.Length; i++)
            costs[i] = 1f;
        return costs;
    }

    public static float GetAreaCost(int areaIndex)
    {
        if (areaIndex < 0 || areaIndex >= AreaCosts.Length)
            return 1f;

        return Mathf.Max(1f, AreaCosts[areaIndex]);
    }

    public static float GetCostForAreaMask(int areaMask)
    {
        if (areaMask == 0)
            return 1f;

        float maxCost = 1f;

        for (int i = 0; i < 32; i++)
        {
            if ((areaMask & (1 << i)) == 0)
                continue;

            maxCost = Mathf.Max(maxCost, GetAreaCost(i));
        }

        return maxCost;
    }

    public static void SetAreaCost(int areaIndex, float cost)
    {
        if (areaIndex < 0 || areaIndex >= AreaCosts.Length)
            return;

        float clamped = Mathf.Max(1f, cost);
        AreaCosts[areaIndex] = clamped;
        NavMesh.SetAreaCost(areaIndex, clamped);
    }

    public static void SetAreaCostByName(string areaName, float cost)
    {
        if (string.IsNullOrWhiteSpace(areaName))
            return;

        int areaIndex = NavMesh.GetAreaFromName(areaName);
        if (areaIndex < 0)
        {
            Debug.LogWarning($"[NavMeshAreaCostService] Area \"{areaName}\" не найдена. Добавь её в Navigation Areas.");
            return;
        }

        SetAreaCost(areaIndex, cost);
    }

    public static void ApplyToAgent(NavMeshAgent agent)
    {
        if (agent == null)
            return;

        for (int i = 0; i < AreaCosts.Length; i++)
            agent.SetAreaCost(i, AreaCosts[i]);
    }

    public static void ApplyToAllAgents()
    {
        NavMeshAgent[] agents = UnityEngine.Object.FindObjectsByType<NavMeshAgent>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        for (int i = 0; i < agents.Length; i++)
            ApplyToAgent(agents[i]);
    }
}
