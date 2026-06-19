using UnityEngine;

/// <summary>
/// Временно ограничивает gameplay-ввод на guided-шагах tutorial.
/// </summary>
public static class TutorialInputGuard
{
    private static TutorialStepDefinition _activeDefinition;
    private static bool _isActive;
    private static Component _allowedSelectionRoot;

    public static bool IsActive => _isActive;

    public static void Apply(TutorialStepType stepType, Component allowedSelectionRoot = null)
    {
        _activeDefinition = TutorialStepDefinition.For(stepType);
        _isActive = true;
        _allowedSelectionRoot = allowedSelectionRoot;
    }

    public static void Clear()
    {
        _isActive = false;
        _allowedSelectionRoot = null;
    }

    public static bool AllowsWorldGameplayInput()
    {
        if (!_isActive)
            return true;

        return _activeDefinition.AllowsWorldGameplayInput;
    }

    public static bool AllowsSelectionOf(UnitSelectable selectable)
    {
        if (!_isActive || selectable == null)
            return true;

        if (_allowedSelectionRoot != null)
            return IsSelectableUnderRoot(selectable, _allowedSelectionRoot);

        return _activeDefinition.AllowsSelectionWithoutRoot;
    }

    public static bool AllowsClearSelection()
    {
        if (!_isActive)
            return true;

        return _activeDefinition.AllowsClearSelection;
    }

    public static bool AllowsArmyMoveCommand()
    {
        if (!_isActive)
            return true;

        return _activeDefinition.AllowsArmyMoveCommand;
    }

    public static bool AllowsEnemyInspect()
    {
        if (!_isActive)
            return true;

        return _activeDefinition.AllowsEnemyInspect;
    }

    public static bool AllowsAttackCommand()
    {
        if (!_isActive)
            return true;

        return _activeDefinition.AllowsAttackCommand;
    }

    public static bool AllowsDemolishBuilding()
    {
        if (!_isActive)
            return true;

        return _activeDefinition.AllowsDemolishBuilding;
    }

    private static bool IsSelectableUnderRoot(UnitSelectable selectable, Component root)
    {
        if (selectable == null || root == null)
            return false;

        Transform selectableTransform = selectable.transform;
        Transform rootTransform = root.transform;

        return selectableTransform == rootTransform ||
               selectableTransform.IsChildOf(rootTransform) ||
               rootTransform.IsChildOf(selectableTransform);
    }
}
