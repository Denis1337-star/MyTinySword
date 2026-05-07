/// <summary>
/// Фабрика job-логики для worker'ов.
/// Создаёт конкретную job по типу работы и передаёт ей ResourceSearchService.
/// </summary>
public sealed class WorkerJobFactory
{
    private readonly ResourceRegistry _resourceRegistry;

    public WorkerJobFactory(ResourceRegistry resourceRegistry)
    {
        _resourceRegistry = resourceRegistry;
    }

    public IWorkerJob Create(WorkerJobType type)
    {
        return type switch
        {
            WorkerJobType.ChopWood => new ChopWoodJob(_resourceRegistry),
            WorkerJobType.MineGold => new MineGoldJob(_resourceRegistry),
            WorkerJobType.HuntMeat => new HuntMeatJob(_resourceRegistry),
            _ => null
        };
    }
}
