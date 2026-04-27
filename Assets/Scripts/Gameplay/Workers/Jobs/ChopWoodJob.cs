using UnityEngine;

/// <summary>
///  работа дровосек 
/// </summary>
public sealed class ChopWoodJob : IWorkerJob
{
    private readonly ResourceSearchService _resourceSearchService;

    public ChopWoodJob(ResourceSearchService resourceSearchService)
    {
        _resourceSearchService = resourceSearchService;
    }

    public WorkerJobType JobType => WorkerJobType.ChopWood;
    public ResourceType RewardType => ResourceType.Wood;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return _resourceSearchService.FindBest<TreeResource>(from);
    }
}
