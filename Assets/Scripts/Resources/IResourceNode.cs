using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceNode 
{
    bool IsAvailable { get; }
    Vector2 WorkPosition { get; }

    ResourceSize Size { get; }  

    void StartWork(System.Action onFinished);

}
