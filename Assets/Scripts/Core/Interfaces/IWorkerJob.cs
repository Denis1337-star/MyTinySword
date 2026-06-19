using UnityEngine;

/// <summary>
/// Интерфейс логики работы worker
/// </summary>
public interface IWorkerJob
{
    WorkerJobType JobType { get; }  
    ResourceType RewardType { get; } 

    // Находит подходящий ресурс для этой работы, начиная поиск от указанной позиции
    ResourceNodeBase FindResource(Vector2 from);  
}

