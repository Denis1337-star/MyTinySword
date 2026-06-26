using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

/// <summary>
/// утилита для проверки обязательных ссылок 
/// </summary>
public static class ValidationUtility
{
    private const string UnknownOwnerName = "Unknown Owner";

    /// <summary>
    /// проверяет что обязательная ссылка назначена
    /// </summary>
    public static bool IsAssigned(Object owner, Object value, string fieldName)
    {
        if (value != null)
            return true;

        LogError(owner, fieldName, "не назначено");
        return false;
    }

    /// <summary>
    /// проверяет что конфиг назначен и проходит IsValid()
    /// </summary>
    public static bool IsValidConfig(Object owner, BaseConfig config, string fieldName)
    {
        if (!IsAssigned(owner, config, fieldName))
            return false;

        return config.IsValid();
    }

    /// <summary>
    /// проверяет что массив существует и содержит хотя бы один элемент
    /// </summary>
    public static bool NotEmptyArray<T>(Object owner, T[] array, string fieldName)
    {
        if (array != null && array.Length > 0)
            return true;

        LogError(owner, fieldName, "массив пустой или не назначен");
        return false;
    }

    /// <summary>
    /// проверяет что список существует и содержит хотя бы один элемент
    /// </summary>
    public static bool NotEmptyList<T>(Object owner, IReadOnlyList<T> list, string fieldName)
    {
        if (list != null && list.Count > 0)
            return true;

        LogError(owner, fieldName, "список пустой или не назначен");
        return false;
    }

    /// <summary>
    /// проверяет что массив назначен, не пустой и не содержит пустых элементов
    /// </summary>
    public static bool ValidArray<T>(Object owner, T[] array, string fieldName)
        where T : Object
    {
        bool valid = NotEmptyArray(owner, array, fieldName);

        if (array == null)
            return false;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
                continue;

            LogError(owner, $"{fieldName}[{i}]", "не назначено");
            valid = false;
        }

        return valid;
    }

    /// <summary>
    /// проверяет что список назначен, не пустой и не содержит пустых элементов
    /// </summary>
    public static bool ValidList<T>(Object owner, IReadOnlyList<T> list, string fieldName)
        where T : Object
    {
        bool valid = NotEmptyList(owner, list, fieldName);

        if (list == null)
            return false;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
                continue;

            LogError(owner, $"{fieldName}[{i}]", "не назначено");
            valid = false;
        }

        return valid;
    }

    private static void LogError(Object owner, string fieldName, string reason)
    {
        string ownerName = owner != null
            ? owner.name
            : UnknownOwnerName;

        Debug.LogError($"{ownerName}: обязательное поле \"{fieldName}\" {reason}.", owner);
    }
}