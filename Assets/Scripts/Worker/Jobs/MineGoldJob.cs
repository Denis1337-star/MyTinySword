using UnityEngine;

public class MineGoldJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.MineGold;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<GoldResource>(from);
    }

    public void GiveReward(int amount)
    {
        ResourceStorage.Instance.AddGold(amount);
    }
}
