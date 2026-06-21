using System;

/// <summary>
/// Требование для открытия ноды дерева развития
/// </summary>
[Serializable]
public sealed class TechTreeRequirement
{
    public TechTreeNodeConfig RequiredNode;
    public int RequiredLevel = 1;
}