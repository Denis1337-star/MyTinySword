using UnityEngine;

/// <summary>
/// Поиск компонентов на том же объекте, что и UnitSelectable / collider hit.
/// </summary>
public static class SelectableUtility
{
    public static T FindNear<T>(UnitSelectable selectable) where T : Component
    {
        if (selectable == null)
            return null;

        return selectable.GetComponent<T>();
    }

    public static UnitSelectable FindFromHit(Collider2D hit)
    {
        if (hit == null)
            return null;

        return hit.GetComponent<UnitSelectable>();
    }

    public static UnitSelectable FindFromComponent(Component component)
    {
        if (component == null)
            return null;

        return component.GetComponent<UnitSelectable>();
    }
}
