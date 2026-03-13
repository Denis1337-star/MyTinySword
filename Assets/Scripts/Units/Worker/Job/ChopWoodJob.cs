using UnityEngine;

public class ChopWoodJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.ChopWood;
    public ResourceType RewardType => ResourceType.Wood;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<TreeResource>(from);
    }
}
