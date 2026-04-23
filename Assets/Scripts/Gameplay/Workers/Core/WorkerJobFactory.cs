using UnityEngine;

/// <summary>
/// Простая фабрика, создающая объект логики работы worker'а по enum-типу профессии
/// </summary>
public static class WorkerJobFactory
{
    /// <summary>
    /// Создаёт стратегию работы для указанного типа профессии
    /// </summary>
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
