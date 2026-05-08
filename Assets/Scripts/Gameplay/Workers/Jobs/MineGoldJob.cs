using UnityEngine;

/// <summary>
/// Job логика добычи золота
/// </summary>
public sealed class MineGoldJob : IWorkerJob
{
    private readonly ResourceRegistry _resourceRegistry;

    public MineGoldJob(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    public WorkerJobType JobType => WorkerJobType.MineGold;
    public ResourceType RewardType => ResourceType.Gold;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return _resourceRegistry.FindBest<GoldResource>(from);
    }
}