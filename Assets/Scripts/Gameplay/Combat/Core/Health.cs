using System;
using UnityEngine;

/// <summary>
///  компонент здоровья
/// </summary>
public class Health : ValidatedMonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth;

    private int currentHealth;
    private bool died;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => died || currentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    protected override void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        died = false;

        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        if (maxHealth >= 1)
            return true;

        Debug.LogError($"{name}: maxHealth must be at least 1.", this);
        return false;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        if (IsDead)
            return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        if (IsDead)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        died = false;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (died)
            return;

        died = true;
        currentHealth = 0;

        OnDied?.Invoke();
    }
}
