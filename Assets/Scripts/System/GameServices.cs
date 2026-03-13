using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServices : MonoBehaviour
{
    public static GameServices Instance { get; private set; }

    [Header("Scene References")]
    [SerializeField] private SelectionSystem selection;
    [SerializeField] private ResourceStorage resources;
    [SerializeField] private WorkerRegistry workers;
    [SerializeField] private ResourceRegistry resourceNodes;
    [SerializeField] private CameraFocusController cameraFocus;

    public SelectionSystem Selection => selection;
    public ResourceStorage Resources => resources;
    public WorkerRegistry Workers => workers;
    public ResourceRegistry ResourceNodes => resourceNodes;
    public CameraFocusController CameraFocus => cameraFocus;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResolveMissingReferences();
    }

    private void ResolveMissingReferences()
    {
        if (selection == null)
            selection = FindObjectOfType<SelectionSystem>();

        if (resources == null)
            resources = FindObjectOfType<ResourceStorage>();

        if (workers == null)
            workers = FindObjectOfType<WorkerRegistry>();

        if (resourceNodes == null)
            resourceNodes = FindObjectOfType<ResourceRegistry>();

        if (cameraFocus == null)
            cameraFocus = FindObjectOfType<CameraFocusController>();
    }
}
