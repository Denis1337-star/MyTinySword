using UnityEngine;

public class ChopWoodJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.ChopWood;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<TreeResource>(from);
    }

    public void GiveReward(int amount)
    {
        ResourceStorage.Instance.AddWood(amount);
    }
}
