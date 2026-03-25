using UnityEngine;

/// <summary>
/// Базовый класс для компонентов сцены, которые должны проверять свою настройку при запуске
/// Если проверка не проходит, компонент отключается.
/// </summary>
public abstract class ValidatedMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// Выполняет базовую валидацию компонента при инициализации
    /// Наследники могут расширять Awake, но должны вызывать base.Awake().
    /// </summary>
    protected virtual void Awake()
    {
        if (!ValidateInternal())
        {
            Debug.LogError($"{name}: validation failed in {GetType().Name}", this);
            enabled = false;
        }
    }

    // Должен вернуть true, если компонент настроен корректно
    protected abstract bool ValidateInternal();
}
