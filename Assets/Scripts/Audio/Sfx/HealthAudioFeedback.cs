using UnityEngine;
using Zenject;

/// <summary>
/// Проигрывает world звуки урона и смерти для объекта с Health
/// </summary>
public sealed class HealthAudioFeedback : ValidatedMonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Health _health;

    [Header("Sounds")]
    [SerializeField] private SoundId _damageSoundId = SoundId.UnitDamaged;
    [SerializeField] private SoundId _deathSoundId = SoundId.UnitDied;

    [Header("Damage Settings")]
    [SerializeField] private bool _playDamageSound = true;
    [SerializeField] private bool _playDeathSound = true;

    [Tooltip("Минимальная пауза между звуками урона")]
    [SerializeField, Min(0f)] private float _damageSoundCooldown = 0.08f;

    private GameAudioService _audioService;

    private int _lastHealth;
    private float _nextDamageSoundTime;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    private void OnEnable()
    {
        _lastHealth = _health.CurrentHealth;

        _health.OnHealthChanged += HandleHealthChanged;
        _health.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
        _health.OnDied -= HandleDied;
    }

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _health, nameof(_health));
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (currentHealth < _lastHealth)
            TryPlayDamageSound(currentHealth);

        _lastHealth = currentHealth;
    }

    private void HandleDied()
    {
        if (!IsDeathSoundAllowed())
            return;

        PlayWorldSound(_deathSoundId);
    }

    private void TryPlayDamageSound(int currentHealth)
    {
        if (!IsDamageSoundAllowed())
            return;

        if (currentHealth <= 0 || _health.IsDead)
            return;

        if (Time.time < _nextDamageSoundTime)
            return;

        _nextDamageSoundTime = Time.time + _damageSoundCooldown;

        PlayWorldSound(_damageSoundId);
    }

    private bool IsDamageSoundAllowed()
    {
        if (!_playDamageSound)
            return false;

        if (_damageSoundId == SoundId.None)
            return false;

        return true;
    }

    private bool IsDeathSoundAllowed()
    {
        if (!_playDeathSound)
            return false;

        if (_deathSoundId == SoundId.None)
            return false;

        return true;
    }

    private void PlayWorldSound(SoundId soundId)
    {
        _audioService.PlayWorldSound(soundId, transform.position);
    }
}