using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Связывает gameplay-логику юнита с Animator
/// </summary>
public class UnitAnimatorBridge : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    [FormerlySerializedAs("animator")]
    [SerializeField] private Animator _animator;

    [FormerlySerializedAs("movement")]
    [SerializeField] private UnitMovement _movement;

    private bool _lastIsMoving;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        UpdateMovingState();
    }

    public void PlayAttack()
    {
        if (_animator == null)
            return;

        _animator.SetTrigger(AttackHash);
    }

    private void UpdateMovingState()
    {
        if (_animator == null || _movement == null)
            return;

        bool isMoving = _movement.IsMoving;

        if (_lastIsMoving == isMoving)
            return;

        _lastIsMoving = isMoving;
        _animator.SetBool(IsMovingHash, isMoving);
    }

    private void ResolveReferences()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_movement == null)
            _movement = GetComponent<UnitMovement>();
    }
}
