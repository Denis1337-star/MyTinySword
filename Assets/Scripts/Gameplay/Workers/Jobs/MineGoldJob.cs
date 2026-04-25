using UnityEngine;

/// <summary>
/// работа шахтер
/// </summary>
public class MineGoldJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.MineGold;
    public ResourceType RewardType => ResourceType.Gold;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<GoldResource>(from);
    }
}
