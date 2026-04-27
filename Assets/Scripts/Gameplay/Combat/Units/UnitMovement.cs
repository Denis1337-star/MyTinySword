using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private const float DefaultStoppingDistance = 0.2f;
    private const float InitialNavMeshSearchRadius = 5f;
    private const float TargetNavMeshSearchRadius = 3f;
    private const float MovingVelocityThreshold = 0.01f;

    private NavMeshAgent _agent;

    public bool HasTarget
    {
        get
        {
            if (!CanUseAgent())
                return false;

            if (_agent.pathPending)
                return true;

            return _agent.hasPath &&
                   _agent.remainingDistance > _agent.stoppingDistance;
        }
    }

    public bool IsMoving
    {
        get
        {
            if (!CanUseAgent())
                return false;

            if (_agent.pathPending)
                return true;

            return _agent.velocity.sqrMagnitude > MovingVelocityThreshold;
        }
    }

    public Vector2 Velocity
    {
        get
        {
            if (_agent == null)
                return Vector2.zero;

            return _agent.velocity;
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        ConfigureAgent();
        PlaceOnNavMesh();
    }

    /// <summary>
    /// Отправляет юнита к ближайшей валидной точке NavMesh рядом с указанной позицией
    /// </summary>
    public void MoveTo(Vector2 position)
    {
        if (!EnsureAgentOnNavMesh())
            return;

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, TargetNavMeshSearchRadius, NavMesh.AllAreas))
            return;

        _agent.SetDestination(hit.position);
    }

    /// <summary>
    /// Останавливает движение и сбрасывает текущий путь
    /// </summary>
    public void Stop()
    {
        if (!CanUseAgent())
            return;

        _agent.ResetPath();
    }

    private void ConfigureAgent()
    {
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = DefaultStoppingDistance;
    }

    private bool EnsureAgentOnNavMesh()
    {
        if (CanUseAgent())
            return true;

        PlaceOnNavMesh();

        return CanUseAgent();
    }

    private void PlaceOnNavMesh()
    {
        if (_agent == null)
            return;

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, InitialNavMeshSearchRadius, NavMesh.AllAreas))
            return;

        _agent.Warp(hit.position);
    }

    private bool CanUseAgent()
    {
        return _agent != null && _agent.isOnNavMesh;
    }
}
