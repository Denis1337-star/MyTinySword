using System;
using UnityEngine;

public interface IResourceNode
{
    bool IsAvailable { get; }
    Vector2 WorkPosition { get; }
    int Priority { get; }

    void StartWork(Action<int> onFinished);
}
public enum ResourceSize
{
    Tiny = 1,
    Small = 2,
    Medium = 3,
    Large = 4,
    Huge = 5,
    Giant = 6
}
