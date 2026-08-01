using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public sealed class UnitMovement : ValidatedMonoBehaviour
{
    private const float DefaultSpeed = 3.5f;

    [Header("Agent Settings")]
    [SerializeField, Min(0f)] private float _stoppingDistance = 0.2f;
    [SerializeField, Min(0.01f)] private float _agentRadius = 0.25f;
    [SerializeField] private int _areaMask = UnityEngine.AI.NavMesh.AllAreas;

    [SerializeField] private NavMeshAgent _agent;

    private const float InitialNavMeshSearchRadius = 5f;
    private const float TargetNavMeshSearchRadius = 3f;
    private const float MovingVelocitySqrThreshold = 0.01f;

    /// <summary>
    /// Задаётся через SetSpeed: ArmyUnit — из UnitConfig, Worker — prefab-default + tech tree.
    /// </summary>
    private float _speed = DefaultSpeed;

    public float Speed => _speed;

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

    /// <summary>
    /// Путь построить нельзя — агент не должен вечно ждать HasTarget.
    /// </summary>
    public bool HasFailedPath
    {
        get
        {
            if (!CanUseAgent())
                return true;

            if (_agent.pathPending)
                return false;

            return _agent.pathStatus == NavMeshPathStatus.PathInvalid;
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

            return _agent.velocity.sqrMagnitude > MovingVelocitySqrThreshold;
        }
    }

    public Vector2 Velocity => _agent.velocity;

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        ConfigureAgent();
        PlaceOnNavMesh();
    }

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _agent, nameof(_agent));
    }

    public bool MoveTo(Vector2 position)
    {
        if (!EnsureAgentOnNavMesh())
            return false;

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, TargetNavMeshSearchRadius, _areaMask))
            return false;

        return _agent.SetDestination(hit.position);
    }

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

        _agent.acceleration = 80f;
        _agent.angularSpeed = 720f;
        _agent.autoBraking = true;
        _agent.autoRepath = true;

        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        _agent.avoidancePriority = UnityEngine.Random.Range(30, 70);
        _agent.areaMask = _areaMask;

        // Подхватывает Dangerous и другие Area Cost со сцены.
        NavMeshAreaCostService.ApplyToAgent(_agent);
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
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, InitialNavMeshSearchRadius, _areaMask))
            return;

        _agent.Warp(hit.position);
    }

    private bool CanUseAgent()
    {
        return _agent.isOnNavMesh;
    }

    public void SetSpeed(float speed)
    {
        _speed = Mathf.Max(0.1f, speed);
        _agent.speed = _speed;
    }

    /// <summary>
    /// Unity: меньшее значение = выше приоритет избегания (0..99).
    /// </summary>
    public void SetAvoidancePriority(int priority)
    {
        if (_agent == null)
            return;

        _agent.avoidancePriority = Mathf.Clamp(priority, 0, 99);
    }

    /// <summary>
    /// false — агент не объезжает других NavMeshAgent (проходит «сквозь»).
    /// </summary>
    public void SetAvoidOtherAgents(bool avoid)
    {
        if (_agent == null)
            return;

        _agent.obstacleAvoidanceType = avoid
            ? ObstacleAvoidanceType.HighQualityObstacleAvoidance
            : ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    public void SetStoppingDistance(float stoppingDistance)
    {
        _stoppingDistance = Mathf.Max(0f, stoppingDistance);
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
