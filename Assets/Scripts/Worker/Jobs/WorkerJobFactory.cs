using UnityEngine;

public static class WorkerJobFactory
{
    public static IWorkerJob Create(WorkerJobType type)
    {
        return type switch
        {
            WorkerJobType.ChopWood => new ChopWoodJob(),
            WorkerJobType.MineGold => new MineGoldJob(),
            WorkerJobType.HuntMeat => new HuntMeatJob(),
            _ => null
        };
    }
}
