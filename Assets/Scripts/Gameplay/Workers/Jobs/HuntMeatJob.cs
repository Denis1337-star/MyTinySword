using UnityEngine;


/// <summary>
/// Job-логика охоты на овцу.
/// »щет SheepResource и говорит worker'у, что награда Ч Meat.
/// </summary>
public sealed class HuntMeatJob : IWorkerJob
{
    private readonly ResourceRegistry _resourceRegistry;

    public HuntMeatJob(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    public WorkerJobType JobType => WorkerJobType.HuntMeat;
    public ResourceType RewardType => ResourceType.Meat;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        if (_resourceRegistry == null)
            return null;

        return _resourceRegistry.FindBest<SheepResource>(from);
    }
}
