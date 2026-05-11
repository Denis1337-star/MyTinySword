using UnityEngine;

/// <summary>
/// Job логика охоты на овцу
/// </summary>
public sealed class HuntMeatJob : IWorkerJob
{
    private readonly ResourceRegistry _resourceRegistry;

    public HuntMeatJob(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    public WorkerJobType JobType => WorkerJobType.HuntMeat;
    public ResourceType RewardType => ResourceType.Meat;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return _resourceRegistry.FindBest<SheepResource>(from);
    }
}