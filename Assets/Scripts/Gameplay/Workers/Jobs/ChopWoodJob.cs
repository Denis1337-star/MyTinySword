using UnityEngine;

/// <summary>
/// JoZ логика рубки дерева
/// </summary>
public sealed class ChopWoodJob : IWorkerJob
{
    private readonly ResourceRegistry _resourceRegistry;

    public ChopWoodJob(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    public WorkerJobType JobType => WorkerJobType.ChopWood;
    public ResourceType RewardType => ResourceType.Wood;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return _resourceRegistry.FindBest<TreeResource>(from);
    }
}