using UnityEngine;

/// <summary>
/// Базовый класс для всех конфигов проекта
/// </summary>
public abstract class BaseConfig : ScriptableObject
{
    /// <summary>
    /// Проверяет конфиг настроен корректно
    /// </summary>
    public abstract bool IsValid();
}