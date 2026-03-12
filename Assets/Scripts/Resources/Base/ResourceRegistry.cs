using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRegistry : MonoBehaviour
{
    public static ResourceRegistry Instance { get; private set; }

    private readonly List<IResourceNode> nodes = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(IResourceNode node)
    {
        if (!nodes.Contains(node))
            nodes.Add(node);
    }

    public void Unregister(IResourceNode node)
    {
        nodes.Remove(node);
    }

    public IReadOnlyList<IResourceNode> Nodes => nodes;
}
