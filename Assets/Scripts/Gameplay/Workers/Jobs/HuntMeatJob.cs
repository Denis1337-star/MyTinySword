using UnityEngine;


/// <summary>
/// Job-логика охоты на овцу.
/// »щет SheepResource и говорит worker'у, что награда Ч Meat.
/// </summary>
public sealed class HuntMeatJob : IWorkerJob
{
    private readonly ResourceSearchService _resourceSearchService;

    public HuntMeatJob(ResourceSearchService resourceSearchService)
    {
        _resourceSearchService = resourceSearchService;
    }

    public WorkerJobType JobType => WorkerJobType.HuntMeat;
    public ResourceType RewardType => ResourceType.Meat;

    public ResourceNodeBase FindResource(Vector2 from)
    {
        if (_resourceSearchService == null)
            return null;

        return _resourceSearchService.FindBest<SheepResource>(from);
    }
}
