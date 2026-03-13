using UnityEngine;

public class HuntMeatJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.HuntMeat;
    public ResourceType RewardType => ResourceType.Meat;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<SheepResource>(from);
    }
}
