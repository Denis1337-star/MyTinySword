using UnityEngine;

/// <summary>
/// Связывает боевого юнита с Animator
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnitMovement))]
public sealed class UnitAnimatorBridge : ValidatedMonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    [SerializeField] private Animator _animator;
    [SerializeField] private UnitMovement _movement;

    private bool _lastIsMoving;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _animator, nameof(_animator));
        valid &= ValidationUtility.IsAssigned(this, _movement, nameof(_movement));

        return valid;
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
