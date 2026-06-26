using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Перемещает овцу по случайным точкам внутри территории
/// </summary>
[RequireComponent(typeof(UnitMovement))]
[RequireComponent(typeof(NavMeshAgent))]
public sealed class SheepAI : MonoBehaviour
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

        if (!_isEating && IsAtTarget())
        {
            _isEating = true;
            _timer = eatTime;
        }

        if (!_isEating)
            return;

        _timer -= Time.deltaTime;

        if (_timer > 0f)
            return;

        _isEating = false;
        MoveToNewPoint();
    }

    public void SetFrozen(bool value)
    {
        if (_isFrozen == value)
            return;

        _isFrozen = value;

        if (value)
        {
            _movement.Stop();
            SetMovingAnimation(false);
            return;
        }

        MoveToNewPoint();
    }

    private bool IsAtTarget()
    {
        Vector2 position = transform.position;
        return (position - _targetPoint).sqrMagnitude < ArriveDistanceSqr;
    }

    private void MoveToNewPoint()
    {
        if (territory == null)
            return;

        for (int i = 0; i < 5; i++)
        {
            Vector2 random = territory.GetRandomPoint();

            if (!NavMesh.SamplePosition(random, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                continue;

            _targetPoint = hit.position;
            _movement.MoveTo(hit.position);
            return;
        }

        Debug.LogWarning($"{name}: не найдена подходящая NavMesh-точка внутри территории.", this);
    }

    private void UpdateMovingAnimation()
    {
        bool isMoving = _movement.HasTarget;
        SetMovingAnimation(isMoving);
    }

    private void SetMovingAnimation(bool isMoving)
    {
        if (_lastIsMoving == isMoving)
            return;

        _lastIsMoving = isMoving;
        _animator.SetBool(IsMovingHash, isMoving);
    }
}