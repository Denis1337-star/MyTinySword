using UnityEngine;

public static class ValidationUtility
{
    public static bool Required(Object owner, Object value, string fieldName)
    {
        if (value != null)
            return true;

        Debug.LogError($"{owner.name}: required field '{fieldName}' is missing", owner);
        return false;
    }

    public static bool NotEmptyArray(Object owner, Object[] array, string fieldName)
    {
        if (array != null && array.Length > 0)
            return true;

        Debug.LogError($"{owner.name}: required array '{fieldName}' is empty or null", owner);
        return false;
    }
}
