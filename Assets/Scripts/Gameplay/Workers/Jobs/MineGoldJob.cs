using UnityEngine;

/// <summary>
/// Job-логика добычи золота.
/// »щет GoldResource и говорит worker'у, что награда Ч Gold.
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
        if (_resourceSearchService == null)
            return null;

        return _resourceSearchService.FindBest<GoldResource>(from);
    }
}
