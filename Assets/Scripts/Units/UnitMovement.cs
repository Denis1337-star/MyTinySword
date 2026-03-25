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
            if (agent == null || !agent.isOnNavMesh)
                return false;

            if (agent.pathPending)
                return true;

            return agent.hasPath && agent.remainingDistance > agent.stoppingDistance;
        }
    }
    public bool IsMoving
    {
        get
        {
            if (agent == null || !agent.isOnNavMesh)
                return false;

            if (agent.pathPending)
                return true;

            return agent.velocity.sqrMagnitude > 0.01f;
        }
    }
    public Vector2 Velocity
    {
        get
        {
            if (agent == null)
                return Vector2.zero;

            return agent.velocity;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0.2f;

        PlaceOnNavMesh();
    }

    private void PlaceOnNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    public void MoveTo(Vector2 position)
    {
        if (!agent.isOnNavMesh)
        {
            PlaceOnNavMesh();
            if (!agent.isOnNavMesh)
                return;
        }

        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void Stop()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.ResetPath();
    }

}
