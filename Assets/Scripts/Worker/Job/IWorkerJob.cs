using UnityEngine;

/// <summary>
/// Интерфейс логики работы worker
/// Определяет тип работы, тип награды и способ поиска подходящего ресурса
/// </summary>
public interface IWorkerJob
{
    WorkerJobType JobType { get; }  // Enum-тип текущей работы
    ResourceType RewardType { get; }  // Тип ресурса, который эта работа приносит

    // Находит подходящий ресурс для этой работы, начиная поиск от указанной позиции
    ResourceNodeBase FindResource(Vector2 from);  
}
public enum WorkerJobType
{
    None,
    ChopWood,
    MineGold,
    HuntMeat
}
