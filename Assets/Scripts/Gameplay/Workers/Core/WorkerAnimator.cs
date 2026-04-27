using UnityEngine;

public class WorkerAnimator : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsWorkingHash = Animator.StringToHash("IsWorking");
    private static readonly int EquipmentHash = Animator.StringToHash("Equipment");

    [SerializeField] private Animator _animator;
    [SerializeField] private UnitMovement _movement;

    private bool _isWorking;
    private bool _lastIsMoving;
    private EquipmentType _currentEquipment = EquipmentType.None;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        UpdateMovingState();
    }

    /// <summary>
    /// Включает или выключает рабочую анимацию
    /// </summary>
    public void SetWorking(bool value)
    {
        if (_isWorking == value)
            return;

        _isWorking = value;
        _animator.SetBool(IsWorkingHash, value);

        if (!value)
            return;

        _lastIsMoving = false;
        _animator.SetBool(IsMovingHash, false);
    }

    /// <summary>
    /// Меняет визуальный инструмент или переносимый груз рабочего
    /// </summary>
    public void SetEquipment(EquipmentType equipment)
    {
        if (_currentEquipment == equipment)
            return;

        _currentEquipment = equipment;
        _animator.SetFloat(EquipmentHash, (float)equipment);
    }

    private void UpdateMovingState()
    {
        if (_animator == null)
            return;

        bool isMoving = _movement != null &&
                        _movement.IsMoving &&
                        !_isWorking;

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

