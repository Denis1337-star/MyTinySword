using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ќбщий реестр всех ресурсов на сцене
/// ѕозвол€ет worker-системе находить доступные ресурсы через единый список
/// </summary>
public class ResourceRegistry : MonoBehaviour
{
    public static ResourceRegistry Instance { get; private set; }

    private readonly List<IResourceNode> nodes = new();  // ¬нутренний список зарегистрированных ресурсов
    public IReadOnlyList<IResourceNode> Nodes => nodes;  // ѕубличный доступ к списку ресурсов только дл€ чтени€

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // –егистрирует ресурс в общем списке
    public void Register(IResourceNode node)
    {
        if (node == null)
            return;

        if (!nodes.Contains(node))
            nodes.Add(node);
    }

    // –егистрирует ресурс в общем списке
    public void Unregister(IResourceNode node)
    {
        if (node == null)
            return;

        nodes.Remove(node);
    }
}
