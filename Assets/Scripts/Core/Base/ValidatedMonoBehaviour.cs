using UnityEngine;

/// <summary>
/// Базовый класс для компонентов сцены, которые должны проверять свою настройку при запуске
/// Если проверка не проходит, компонент отключается
/// </summary>
public abstract class ValidatedMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// Выполняет базовую валидацию компонента при инициализации
    /// </summary>
    protected virtual void Awake()
    {
        bool isValid = ValidateInternal();

        if (isValid)
            return;

        Debug.LogError($"{name}: validation failed in {GetType().Name}. Component has been disabled.", this);
        enabled = false;
    }

    // Должен вернуть true, если компонент настроен корректно
    protected abstract bool ValidateInternal();
}
