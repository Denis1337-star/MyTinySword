using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    public bool HasTarget => agent.hasPath;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTo(Vector2 position)
    {
        agent.SetDestination(position);
    }

    public void Stop()
    {
        agent.ResetPath();
    }
}
