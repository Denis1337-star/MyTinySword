using System;
using UnityEngine;

/// <summary>
/// Компонент здоровья
/// </summary>
public sealed class Health : ValidatedMonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth ;

    private int _currentHealth;
    private bool _died;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public bool IsDead => _died || _currentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    protected override void Awake()
    {
        Initialize(_maxHealth);

        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        if (_maxHealth >= 1)
            return true;

        Debug.LogError($"{name}: MaxHealth должен быть минимум 1.", this);
        return false;
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

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        if (IsDead)
            return;

        int newHealth = Mathf.Min(_maxHealth, _currentHealth + amount);

        if (newHealth == _currentHealth)
            return;

        _currentHealth = newHealth;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
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
        OnDied?.Invoke();
    }
}