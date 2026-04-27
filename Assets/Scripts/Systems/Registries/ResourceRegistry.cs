using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// Общий реестр всех ресурсов на сцене
/// </summary>
public class ResourceRegistry : MonoBehaviour
{
    private readonly List<IResourceNode> _nodes = new();

    private readonly Subject<IResourceNode> _nodeAdded = new();
    private readonly Subject<IResourceNode> _nodeRemoved = new();

    public IReadOnlyList<IResourceNode> Nodes => _nodes;

    public IObservable<IResourceNode> NodeAdded => _nodeAdded;
    public IObservable<IResourceNode> NodeRemoved => _nodeRemoved;

    public int Count => _nodes.Count;

    public void Register(IResourceNode node)
    {
        if (node == null)
            return;

        if (_nodes.Contains(node))
            return;

        _nodes.Add(node);
        _nodeAdded.OnNext(node);
    }

    public void Unregister(IResourceNode node)
    {
        if (node == null)
            return;

        if (!_nodes.Remove(node))
            return;

        _nodeRemoved.OnNext(node);
    }

    public bool Contains(IResourceNode node)
    {
        return node != null && _nodes.Contains(node);
    }

    private void OnDestroy()
    {
        _nodeAdded.OnCompleted();
        _nodeRemoved.OnCompleted();

        _nodeAdded.Dispose();
        _nodeRemoved.Dispose();

        _nodes.Clear();
    }
}
