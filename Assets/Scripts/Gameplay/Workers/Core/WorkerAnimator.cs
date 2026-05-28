using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет animator параметрами worker
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnitMovement))]
public sealed class WorkerAnimator : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsWorkingHash = Animator.StringToHash("IsWorking");
    private static readonly int EquipmentHash = Animator.StringToHash("Equipment");

    [SerializeField] private EquipmentWorkSound[] _workSounds;

    private Animator _animator;
    private UnitMovement _movement;
    private GameAudioService _audioService;
    private bool _isWorking;
    private bool _lastIsMoving;
    private EquipmentType _currentEquipment = EquipmentType.None;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<UnitMovement>();
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
    /// Меняет визуальный инструмент 
    /// </summary>
    public void SetEquipment(EquipmentType equipment)
    {
        if (_currentEquipment == equipment)
            return;

        _currentEquipment = equipment;
        _animator.SetFloat(EquipmentHash, (float)equipment);
    }

    /// <summary>
    /// Вызывается Animation Event на кадре удара/работы
    /// </summary>
    public void PlayWorkFrameSound()
    {
        if (!_isWorking)
            return;

        SoundId soundId = GetWorkSoundId(_currentEquipment);

        if (soundId == SoundId.None)
            return;

        _audioService.PlayWorldSound(soundId, transform.position);
    }

    private void UpdateMovingState()
    {
        bool isMoving = _movement.IsMoving && !_isWorking;

        if (_lastIsMoving == isMoving)
            return;

        _lastIsMoving = isMoving;
        _animator.SetBool(IsMovingHash, isMoving);
    }

    private SoundId GetWorkSoundId(EquipmentType equipment)
    {
        if (_workSounds == null || _workSounds.Length == 0)
            return SoundId.None;

        for (int i = 0; i < _workSounds.Length; i++)
        {
            EquipmentWorkSound workSound = _workSounds[i];

            if (workSound.Equipment != equipment)
                continue;

            return workSound.SoundId;
        }

        return SoundId.None;
    }

    [System.Serializable]
    private sealed class EquipmentWorkSound
    {
        [SerializeField] private EquipmentType _equipment;
        [SerializeField] private SoundId _soundId = SoundId.None;

        public EquipmentType Equipment => _equipment;
        public SoundId SoundId => _soundId;
    }
}