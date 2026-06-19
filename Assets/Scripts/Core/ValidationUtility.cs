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

    private static void LogError(Object owner, string fieldName, string reason)
    {
        string ownerName = owner != null
            ? owner.name
            : UnknownOwnerName;

        Debug.LogError($"{ownerName}: обязательное поле \"{fieldName}\" {reason}.", owner);
    }
}