using UnityEngine;

/// <summary>
/// Проигрывает world-звуки урона и смерти для объекта с Health.
/// Звук исходит из позиции объекта, поэтому громкость зависит от расстояния до камеры/AudioListener.
/// </summary>
[RequireComponent(typeof(Health))]
public sealed class HealthAudioFeedback : ValidatedMonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Health _health;

    [Tooltip("Точка, из которой будет исходить звук. Если не назначена — используется transform объекта.")]
    [SerializeField] private Transform _soundOrigin;

    [Header("Sounds")]
    [SerializeField] private SoundId _damageSoundId = SoundId.UnitDamaged;
    [SerializeField] private SoundId _deathSoundId = SoundId.UnitDied;

    [Header("Damage Settings")]
    [SerializeField] private bool _playDamageSound = true;
    [SerializeField] private bool _playDeathSound = true;

    [Tooltip("Минимальная пауза между звуками урона, чтобы частый урон не создавал звуковую кашу.")]
    [SerializeField, Min(0f)] private float _damageSoundCooldown = 0.08f;

    private int _lastHealth;
    private float _nextDamageSoundTime;
    private bool _isInitialized;

    protected override void Awake()
    {
        ResolveReferences();

        base.Awake();
    }

    private void OnEnable()
    {
        if (_health == null)
            return;

        _lastHealth = _health.CurrentHealth;
        _isInitialized = true;

        _health.OnHealthChanged += HandleHealthChanged;
        _health.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        if (_health == null)
            return;

        _health.OnHealthChanged -= HandleHealthChanged;
        _health.OnDied -= HandleDied;
    }

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _health, nameof(_health));
    }

    private void Reset()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (_health == null)
            _health = GetComponent<Health>();

        if (_soundOrigin == null)
            _soundOrigin = transform;
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (!_isInitialized || _health == null)
            return;

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

        // Если урон смертельный, не играем damage-звук.
        // Смерть проиграет отдельный death-звук через OnDied.
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
        GameAudioService audioService = GameAudioService.Instance;

        if (audioService == null)
            return;

        Vector3 position = _soundOrigin != null
            ? _soundOrigin.position
            : transform.position;

        audioService.PlayWorldSound(soundId, position);
    }
}