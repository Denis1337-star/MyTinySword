using UnityEngine;

/// <summary>
/// Job-логика рубки дерева.
/// »щет TreeResource и говорит worker'у, что награда Ч Wood.
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
        if (_resourceSearchService == null)
            return null;

        return _resourceSearchService.FindBest<TreeResource>(from);
    }
}
