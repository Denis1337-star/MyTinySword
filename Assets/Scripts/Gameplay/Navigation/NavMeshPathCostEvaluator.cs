using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Считает стоимость пути по NavMesh с учётом Area Cost.
/// </summary>
public static class NavMeshPathCostEvaluator
{
    private const float SampleRadius = 2f;
    private static readonly NavMeshPath SharedPath = new();

    /// <summary>
    /// true + cost, если есть полный путь. Иначе false.
    /// </summary>
    public static bool TryGetPathCost(
        Vector3 from,
        Vector3 to,
        int areaMask,
        out float pathCost)
    {
        pathCost = float.MaxValue;

        if (!NavMesh.SamplePosition(from, out NavMeshHit fromHit, SampleRadius, areaMask))
            return false;

        if (!NavMesh.SamplePosition(to, out NavMeshHit toHit, SampleRadius, areaMask))
            return false;

        if (!NavMesh.CalculatePath(fromHit.position, toHit.position, areaMask, SharedPath))
            return false;

        if (SharedPath.status != NavMeshPathStatus.PathComplete)
            return false;

        Vector3[] corners = SharedPath.corners;
        if (corners == null || corners.Length < 2)
        {
            pathCost = 0f;
            return true;
        }

        float cost = 0f;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 a = corners[i];
            Vector3 b = corners[i + 1];
            float length = Vector3.Distance(a, b);

            Vector3 mid = (a + b) * 0.5f;
            float areaCost = 1f;

            if (NavMesh.SamplePosition(mid, out NavMeshHit midHit, SampleRadius, areaMask))
                areaCost = NavMeshAreaCostService.GetCostForAreaMask(midHit.mask);

            cost += length * areaCost;
        }

        pathCost = cost;
        return true;
    }
}
