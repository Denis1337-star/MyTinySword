using UnityEngine;

/// <summary>
/// Стратегия работы worker'а по добыче мяса
/// Ищет овцу как ресурс и приносит мясо
/// </summary>
public class HuntMeatJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.HuntMeat;
    public ResourceType RewardType => ResourceType.Meat;

    /// <summary>
    /// Находит лучший доступный ресурс овцы от указанной позиции
    /// </summary>
    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<SheepResource>(from);
    }
}
