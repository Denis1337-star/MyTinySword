using UnityEngine;

public class HuntMeatJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.HuntMeat;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<SheepResource>(from);
    }

    public void GiveReward(int amount)
    {
        ResourceStorage.Instance.AddMeat(amount);
    }
}
