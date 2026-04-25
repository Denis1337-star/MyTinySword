using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Общий реестр всех ресурсов на сцене
/// Позволяет worker находить доступные ресурсы через единый список
/// </summary>
public class ResourceRegistry : MonoBehaviour
{
    public static ResourceRegistry Instance { get; private set; }

    private readonly List<IResourceNode> nodes = new();

    public IReadOnlyList<IResourceNode> Nodes => nodes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Register(IResourceNode node)
    {
        if (node == null)
            return;

        if (nodes.Contains(node))
            return;

        nodes.Add(node);
    }

    public void Unregister(IResourceNode node)
    {
        if (node == null)
            return;

        nodes.Remove(node);
    }
}
