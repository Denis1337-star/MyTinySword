using UnityEngine;

/// <summary>
/// Job-логика добычи золота.
/// »щет GoldResource и говорит worker'у, что награда Ч Gold.
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
        if (_resourceRegistry == null)
            return null;

        return _resourceRegistry.FindBest<GoldResource>(from);
    }
}
