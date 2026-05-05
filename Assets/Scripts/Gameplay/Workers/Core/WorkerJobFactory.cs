/// <summary>
/// Фабрика job-логики для worker'ов.
/// Создаёт конкретную job по типу работы и передаёт ей ResourceSearchService.
/// </summary>
public sealed class WorkerJobFactory
{
    private readonly ResourceSearchService _resourceSearchService;

    public WorkerJobFactory(ResourceSearchService resourceSearchService)
    {
        _resourceSearchService = resourceSearchService;
    }

    public IWorkerJob Create(WorkerJobType type)
    {
        return type switch
        {
            WorkerJobType.ChopWood => new ChopWoodJob(_resourceSearchService),
            WorkerJobType.MineGold => new MineGoldJob(_resourceSearchService),
            WorkerJobType.HuntMeat => new HuntMeatJob(_resourceSearchService),
            _ => null
        };
    }
}
