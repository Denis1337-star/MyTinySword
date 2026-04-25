/// <summary>
///  фабрика  логики работы worker по enum профессии
/// </summary>
public static class WorkerJobFactory
{
    private static readonly IWorkerJob ChopWood = new ChopWoodJob();
    private static readonly IWorkerJob MineGold = new MineGoldJob();
    private static readonly IWorkerJob HuntMeat = new HuntMeatJob();

    public static IWorkerJob Create(WorkerJobType type)
    {
        return type switch
        {
            WorkerJobType.ChopWood => ChopWood,
            WorkerJobType.MineGold => MineGold,
            WorkerJobType.HuntMeat => HuntMeat,
            _ => null
        };
    }
}
