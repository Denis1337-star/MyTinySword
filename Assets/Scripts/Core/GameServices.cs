using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }

    public SelectionSystem Selection { get; private set; }
    public ResourceStorage Resources { get; private set; }
    public WorkerRegistry Workers { get; private set; }
    public ResourceRegistry ResourcesNodes { get; private set; }
    public CameraFocusController CameraFocus { get; private set; }

    private void Awake()
    {
        Instance = this;

        Selection = FindObjectOfType<SelectionSystem>();
        Resources = FindObjectOfType<ResourceStorage>();
        Workers = FindObjectOfType<WorkerRegistry>();
        ResourcesNodes = FindObjectOfType<ResourceRegistry>();
        CameraFocus = FindObjectOfType<CameraFocusController>();
    }
}
