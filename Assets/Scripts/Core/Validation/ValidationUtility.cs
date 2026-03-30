using UnityEngine;

/// <summary>
/// Ќабор вспомогательных методов дл€ проверки об€зательных ссылок и коллекций
/// </summary>
public static class ValidationUtility
{

    // ѕровер€ет, что об€зательна€ ссылка назначена
    public static bool Required(Object owner, Object value, string fieldName)
    {
        if (value != null)
            return true;

        Debug.LogError($"{owner.name}: required field '{fieldName}' is missing", owner);
        return false;
    }

    // ѕровер€ет, что массив существует и содержит хот€ бы один элемент
    public static bool NotEmptyArray(Object owner, Object[] array, string fieldName)
    {
        if (array != null && array.Length > 0)
            return true;

        Debug.LogError($"{owner.name}: required array '{fieldName}' is empty or null", owner);
        return false;
    }
}
