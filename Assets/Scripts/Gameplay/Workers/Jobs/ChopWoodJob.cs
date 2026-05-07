using UnityEngine;

/// <summary>
/// Job-логика рубки дерева.
/// »щет TreeResource и говорит worker'у, что награда Ч Wood.
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
        if (_resourceRegistry == null)
            return null;

        return _resourceRegistry.FindBest<TreeResource>(from);
    }
}
