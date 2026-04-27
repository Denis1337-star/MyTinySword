using UnityEngine;

/// <summary>
///  работа охотник
/// </summary>
public sealed class HuntMeatJob : IWorkerJob
{
    private readonly ResourceSearchService _resourceSearchService;

    public HuntMeatJob(ResourceSearchService resourceSearchService)
    {
        _resourceSearchService = resourceSearchService;
    }

    public WorkerJobType JobType => WorkerJobType.HuntMeat;
    public ResourceType RewardType => ResourceType.Meat;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return _resourceSearchService.FindBest<SheepResource>(from);
    }
}
