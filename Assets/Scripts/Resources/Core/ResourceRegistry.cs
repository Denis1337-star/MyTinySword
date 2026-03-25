using System.Collections.Generic;
using UnityEngine;

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

    public void Register(IResourceNode node)
    {
        if (node == null)
            return;

        if (!nodes.Contains(node))
            nodes.Add(node);
    }

    public void Unregister(IResourceNode node)
    {
        if (node == null)
            return;

        nodes.Remove(node);
    }
}
