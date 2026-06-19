using UnityEngine;

/// <summary>
/// класс для компонентов сцены для проверки настроек при запуске
/// если проверка не проходит компонент отключается
/// </summary>
public abstract class ValidatedMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// проверка компонента при инициализации
    /// </summary>
    protected virtual void Awake()
    {
        bool isValid = ValidateInternal();

        if (isValid)
            return;

        Debug.LogError($"{name}: проверка не пройдена в {GetType().Name}. Компонент отключён.", this);
        enabled = false;
    }

    /// <summary>
    /// вернет true если компонент настроен правильно
    /// </summary>
    protected abstract bool ValidateInternal();
}