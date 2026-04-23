using UnityEngine;

/// <summary>
/// Базовый класс для всех конфигов проекта
/// </summary>
public abstract class BaseConfig : ScriptableObject
{
    // Проверяет, что данные конфига настроены корректно
    public abstract bool IsValid();

    // Для вывода ошибки  
    protected void LogValidationError(string message)
    {
        Debug.LogError($"{name}: {message}", this);
    }
}