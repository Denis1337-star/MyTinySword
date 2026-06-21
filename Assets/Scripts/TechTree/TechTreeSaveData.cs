using System;
using System.Collections.Generic;

/// <summary>
/// Сохранённые данные всего дерева развития
/// </summary>
[Serializable]
public sealed class TechTreeSaveData
{
    public bool Initialized;
    public List<TechTreeNodeSaveData> Nodes = new();
    public long LastKnownUnixTime;
}