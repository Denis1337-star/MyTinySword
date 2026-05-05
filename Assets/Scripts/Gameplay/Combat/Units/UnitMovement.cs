using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    [Header("Agent Settings")]
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _stoppingDistance = 0.2f;
    [SerializeField] private float _agentRadius = 0.25f;
    [SerializeField] private int _areaMask = UnityEngine.AI.NavMesh.AllAreas;

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
    public bool MoveTo(Vector2 position)
    {
        if (!EnsureAgentOnNavMesh())
            return false;

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, TargetNavMeshSearchRadius, _areaMask))
            return false;

        return _agent.SetDestination(hit.position);
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

        _agent.speed = _speed;
        _agent.stoppingDistance = _stoppingDistance;
        _agent.radius = _agentRadius;

        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        _agent.avoidancePriority = UnityEngine.Random.Range(30, 70);
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

    public void SetSpeed(float speed)
    {
        _speed = Mathf.Max(0.1f, speed);

        if (_agent != null)
            _agent.speed = _speed;
    }

    public void SetStoppingDistance(float stoppingDistance)
    {
        _stoppingDistance = Mathf.Max(0f, stoppingDistance);

        if (_agent != null)
            _agent.stoppingDistance = _stoppingDistance;
    }

    public bool TrySamplePosition(Vector2 position, float radius, out Vector3 result)
    {
        result = position;

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, radius, _areaMask))
            return false;

        result = hit.position;
        return true;
    }
}
