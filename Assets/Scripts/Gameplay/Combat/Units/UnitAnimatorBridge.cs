using UnityEngine;

/// <summary>
/// Связывает боевого юнита с Animator
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnitMovement))]
public sealed class UnitAnimatorBridge : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private Animator _animator;
    private UnitMovement _movement;

    private bool _lastIsMoving;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<UnitMovement>();
    }

    private void Update()
    {
        UpdateMovingState();
    }

    public void PlayAttack()
    {
        _animator.SetTrigger(AttackHash);
    }

    private void UpdateMovingState()
    {
        bool isMoving = _movement.IsMoving;

        if (_lastIsMoving == isMoving)
            return;

        _lastIsMoving = isMoving;
        _animator.SetBool(IsMovingHash, isMoving);
    }
}