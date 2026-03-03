using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerRegistry : MonoBehaviour
{
    public static WorkerRegistry Instance { get; private set; }

    public readonly List<Worker> Workers = new();
    public event Action<Worker> OnWorkerAdded;
    public event Action<Worker> OnWorkerRemoved;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void Register(Worker worker)
    {
        Workers.Add(worker);
        OnWorkerAdded?.Invoke(worker);
    }

    public void Unregister(Worker worker)
    {
        Workers.Remove(worker);
        OnWorkerRemoved?.Invoke(worker);
    }
}
