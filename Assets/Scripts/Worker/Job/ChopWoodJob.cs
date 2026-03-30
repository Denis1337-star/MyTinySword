using UnityEngine;

/// <summary>
/// Стратегия работы worker'а по рубке дерева
/// Ищет дерево и приносит древесину
/// </summary>
public class ChopWoodJob : IWorkerJob
{
    public WorkerJobType JobType => WorkerJobType.ChopWood;
    public ResourceType RewardType => ResourceType.Wood;

    /// <summary>
    /// Находит лучший доступный ресурс дерева от указанной позиции
    /// </summary>
    public ResourceNodeBase FindResource(Vector2 from)
    {
        return ResourceFinder.FindBest<TreeResource>(from);
    }
}
