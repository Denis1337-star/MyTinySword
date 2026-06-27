using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Перемещает овцу по случайным точкам внутри территории
/// </summary>
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(NavMeshAgent))]
public sealed class SheepAI : ValidatedMonoBehaviour
{
    private const float ArriveDistance = 0.2f;
    private const float ArriveDistanceSqr = ArriveDistance * ArriveDistance;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    [SerializeField] private float eatTime = 3f;
    [SerializeField] private SheepTerritory territory;
    [SerializeField] private UnitMovement _movement;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    private float _timer;
    private bool _isEating;
    private bool _isFrozen;
    private bool _lastIsMoving;
    private Vector2 _targetPoint;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, territory, nameof(territory));
        valid &= ValidationUtility.IsAssigned(this, _movement, nameof(_movement));
        valid &= ValidationUtility.IsAssigned(this, _animator, nameof(_animator));
        valid &= ValidationUtility.IsAssigned(this, _agent, nameof(_agent));

        return valid;
    }

    private IEnumerator Start()
    {
        while (!_agent.isOnNavMesh)
            yield return null;

        MoveToNewPoint();
    }

    private void Update()
    {
        if (_isFrozen)
            return;

        UpdateMovingAnimation();

        if (_isEating)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                _isEating = false;
                MoveToNewPoint();
            }

            return;
        }

        if (HasArrived())
        {
            StartEating();
            return;
        }

        if (!_movement.HasTarget)
            MoveToNewPoint();
    }

    public void SetFrozen(bool isFrozen)
    {
        _isFrozen = isFrozen;

        if (_isFrozen)
            _movement.Stop();
    }

    private void UpdateMovingAnimation()
    {
        bool isMoving = _movement.IsMoving;

        if (_lastIsMoving == isMoving)
            return;

        _lastIsMoving = isMoving;
        _animator.SetBool(IsMovingHash, isMoving);
    }

    private bool HasArrived()
    {
        Vector2 delta = _targetPoint - (Vector2)transform.position;
        return delta.sqrMagnitude <= ArriveDistanceSqr;
    }

    private void MoveToNewPoint()
    {
        _targetPoint = territory.GetRandomPoint();
        _movement.MoveTo(_targetPoint);
    }

    private void StartEating()
    {
        _isEating = true;
        _timer = eatTime;
        _movement.Stop();
    }
}
