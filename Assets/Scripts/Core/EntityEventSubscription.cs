using System;

/// <summary>
/// Безопасная подписка на события сущности без дублирования subscribe/unsubscribe.
/// </summary>
public sealed class EntityEventSubscription<T> where T : class
{
    private T _entity;

    public T Entity => _entity;

    public bool IsBoundTo(T entity) => ReferenceEquals(_entity, entity);

    public void Bind(T entity, Action<T> subscribe, Action<T> unsubscribe)
    {
        if (ReferenceEquals(_entity, entity))
            return;

        if (_entity != null)
            unsubscribe(_entity);

        _entity = entity;

        if (_entity != null)
            subscribe(_entity);
    }

    public void Clear(Action<T> unsubscribe)
    {
        if (_entity == null)
            return;

        unsubscribe(_entity);
        _entity = null;
    }
}
