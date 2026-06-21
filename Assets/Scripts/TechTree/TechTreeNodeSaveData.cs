using System;

/// <summary>
/// Сохранённое состояние одной ноды дерева развития
/// </summary>
[Serializable]
public sealed class TechTreeNodeSaveData
{
    public string NodeId;
    public int Level;
    public long UpgradeEndUnixTime;

    public bool IsUpgrading => UpgradeEndUnixTime > 0;
}