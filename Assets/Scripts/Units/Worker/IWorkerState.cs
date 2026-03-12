using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorkerState
{
    void Enter();
    void Update();
    void Exit();
}
