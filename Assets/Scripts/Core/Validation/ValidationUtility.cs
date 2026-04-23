using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Набор вспомогательных методов для проверки обязательных ссылок и коллекций
/// </summary>
public static class ValidationUtility
{

    // Проверяет, что обязательная ссылка назначена
    public static bool NotEmptyCollection(Object owner, Object value, string fieldName)
    {
        if (value != null)
            return true;

        string ownerName = owner != null ? owner.name : "Unknown Owner";
        Debug.LogError($"{ownerName}: required field '{fieldName}' is missing.", owner);
        return false;
    }

    // Проверяет, что массив существует и содержит хотя бы один элемент
    public static bool NotEmptyArray(Object owner, Object[] array, string fieldName)
    {
        if (array != null && array.Length > 0)
            return true;

        string ownerName = owner != null ? owner.name : "Unknown Owner";
        Debug.LogError($"{ownerName}: required array '{fieldName}' is empty or null.", owner);
        return false;
    }

    // Проверяет, что список существует и содержит хотя бы один элемент
    public static bool NotEmptyList<T>(Object owner, IReadOnlyList<T> list, string fieldName)
    {
        if (list != null && list.Count > 0)
            return true;

        string ownerName = owner != null ? owner.name : "Unknown Owner";
        Debug.LogError($"{ownerName}: required list '{fieldName}' is empty or null.", owner);
        return false;
    }
}
