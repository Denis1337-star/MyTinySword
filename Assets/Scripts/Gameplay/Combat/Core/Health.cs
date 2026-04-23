using System;
using UnityEngine;

/// <summary>
/// Универсальный компонент здоровья
/// Подходит и для юнитов, и для зданий
/// </summary>
public class Health : ValidatedMonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    /// <summary>
    /// Максимальное здоровье объекта
    /// </summary>
    public int MaxHealth => maxHealth;

    /// <summary>
    /// Текущее здоровье объекта
    /// </summary>
    public int CurrentHealth => currentHealth;

    /// <summary>
    /// Мёртв ли объект
    /// </summary>
    public bool IsDead => currentHealth <= 0;

    /// <summary>
    /// Вызывается при любом изменении здоровья
    /// Передаёт текущее и максимальное значение
    /// </summary>
    public event Action<int, int> OnHealthChanged;

    /// <summary>
    /// Вызывается один раз, когда здоровье заканчивается
    /// </summary>
    public event Action OnDied;

    protected override void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        if (maxHealth < 1)
        {
            Debug.LogError($"{name}: maxHealth must be at least 1", this);
            valid = false;
        }

        return valid;
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
    }

    /// <summary>
    /// Применяет урон к объекту.
    /// Если здоровье падает до нуля, вызывает событие смерти
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        if (IsDead)
            return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
            OnDied?.Invoke();
    }

    /// <summary>
    /// Восстанавливает здоровье
    /// Пока делаем простую базовую версию
    /// пригодится позже для хилера.
    /// </summary>
    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        if (IsDead)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Полностью сбрасывает здоровье до максимума
    /// Полезно для тестов, респавна или переиспользования объекта
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
