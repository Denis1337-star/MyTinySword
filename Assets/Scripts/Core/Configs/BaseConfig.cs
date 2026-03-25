using UnityEngine;

/// <summary>
/// Базовый класс для всех конфигов проекта
/// Наследники должны реализовать проверку своей валидности
/// </summary>
public abstract class BaseConfig : ScriptableObject
{
    // Проверяет, что данные конфига настроены корректно.
    public abstract bool IsValid();
}