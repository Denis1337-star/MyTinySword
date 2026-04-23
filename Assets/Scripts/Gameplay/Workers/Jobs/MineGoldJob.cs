using UnityEngine;

/// <summary>
/// Стратегия работы worker'а по добыче золота
/// Ищет золотой ресурс и приносит золото
/// </summary>
public class MineGoldJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.MineGold;
    public ResourceType RewardType => ResourceType.Gold;

    /// <summary>
    /// Находит лучший доступный золотой ресурс от указанной позиции
    /// </summary>
    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<GoldResource>(from);
    }
}
