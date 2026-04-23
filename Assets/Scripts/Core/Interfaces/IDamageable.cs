/// <summary>
/// Контракт объекта, который может получать урон
/// Нужен для того, чтобы юниты и здания могли быть универсальными целями
/// </summary>
public interface IDamageable
{
    bool IsDead { get; }

    /// <summary>
    /// Применяет урон к объекту
    /// </summary>
    void TakeDamage(int amount);
}
