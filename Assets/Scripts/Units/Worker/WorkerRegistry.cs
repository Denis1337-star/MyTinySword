using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerRegistry : MonoBehaviour
{
    public static WorkerRegistry Instance;

    public readonly List<Worker> Workers = new();

    private void Awake()
    {
        Instance = this;
    }
    public void Register(Worker worker)
    {
        Workers.Add(worker);
    }

    public void Unregister(Worker worker)
    {
        Workers.Remove(worker);
    }
}
