using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool HasTarget
    {
        get
        {
            if (!agent.isOnNavMesh) return false;
            if (agent.pathPending) return true;
            return agent.remainingDistance > agent.stoppingDistance;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.05f;

        PlaceOnNavMesh();
    }

    private void PlaceOnNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogError($"[{name}] NOT ON NAVMESH at start!");
        }
    }

    public void MoveTo(Vector2 position)
    {
        if (!agent.isOnNavMesh)
        {
            PlaceOnNavMesh();
            if (!agent.isOnNavMesh) return;
        }

        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning($"NavMesh point not found near {position}");
        }
    }

    public void Stop()
    {
        if (agent.isOnNavMesh)
            agent.ResetPath();
    }

}
