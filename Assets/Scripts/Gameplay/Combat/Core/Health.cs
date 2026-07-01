using System;
using UnityEngine;
using Zenject;

/// <summary>
/// Компонент здоровья.
/// Отвечает за урон, лечение, смерть и уведомление UI/аудио о смене здоровья.
/// Max HP задаётся только через Initialize() из ArmyUnit или BuildingBase.
/// </summary>
public sealed class Health : MonoBehaviour, IDamageable
{
    [Header("World Audio")]
    [SerializeField] private SoundId _damageSoundId = SoundId.None;
    [SerializeField] private SoundId _deathSoundId = SoundId.None;
    [SerializeField] private bool _playDamageSound = true;
    [SerializeField] private bool _playDeathSound = true;
    [SerializeField, Min(0f)] private float _damageSoundCooldown = 0.08f;

    private GameAudioService _audioService;
    private int _lastHealthForAudio;
    private float _nextDamageSoundTime;

    private int _maxHealth;
    private int _currentHealth;
    private bool _died;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;

    public bool IsDead => _died || _currentHealth <= 0;
    public bool IsFullHealth => _currentHealth >= _maxHealth;
    public bool CanBeHealed => !IsDead && _currentHealth < _maxHealth;

    public float HealthPercent => _maxHealth > 0
        ? (float)_currentHealth / _maxHealth
        : 0f;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    public void Initialize(int maxHealth, bool resetCurrentHealth = true)
    {
        _maxHealth = Mathf.Max(1, maxHealth);

        if (resetCurrentHealth)
        {
            _currentHealth = _maxHealth;
            _died = false;
        }
        else
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        _lastHealthForAudio = _currentHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        if (IsDead)
            return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);

        if (_currentHealth <= 0)
        {
            Die();
            return;
        }

        TryPlayDamageSound(_currentHealth);
        _lastHealthForAudio = _currentHealth;

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    /// <summary>
    /// Лечит объект и возвращает фактическое количество восстановленного здоровья.
    /// Если лечение не применилось, возвращает 0.
    /// </summary>
    public int Heal(int amount)
    {
        if (amount <= 0)
            return 0;

        if (!CanBeHealed)
            return 0;

        int previousHealth = _currentHealth;
        int newHealth = Mathf.Min(_maxHealth, _currentHealth + amount);

        if (newHealth == previousHealth)
            return 0;

        _currentHealth = newHealth;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        return _currentHealth - previousHealth;
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        _died = false;

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    private void Die()
    {
        if (_died)
            return;

        _died = true;
        _currentHealth = 0;

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        TryPlayDeathSound();
        OnDied?.Invoke();
    }

    private void TryPlayDamageSound(int currentHealth)
    {
        if (currentHealth >= _lastHealthForAudio)
            return;

        if (!_playDamageSound || _damageSoundId == SoundId.None)
            return;

        if (currentHealth <= 0 || IsDead)
            return;

        if (Time.time < _nextDamageSoundTime)
            return;

        _nextDamageSoundTime = Time.time + _damageSoundCooldown;
        _audioService?.PlayWorldSound(_damageSoundId, transform.position);
    }

    private void TryPlayDeathSound()
    {
        if (!_playDeathSound || _deathSoundId == SoundId.None)
            return;

        _audioService?.PlayWorldSound(_deathSoundId, transform.position);
    }
}
