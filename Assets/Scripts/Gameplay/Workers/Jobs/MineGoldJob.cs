using UnityEngine;

/// <summary>
/// работа шахтер
/// </summary>
public sealed class MineGoldJob : IWorkerJob
{
    private readonly ResourceSearchService _resourceSearchService;

    public MineGoldJob(ResourceSearchService resourceSearchService)
    {
        _resourceSearchService = resourceSearchService;
    }

    public WorkerJobType JobType => WorkerJobType.MineGold;
    public ResourceType RewardType => ResourceType.Gold;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return _resourceSearchService.FindBest<GoldResource>(from);
    }
}
