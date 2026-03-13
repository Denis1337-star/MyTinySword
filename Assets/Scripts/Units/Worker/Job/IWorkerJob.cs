using UnityEngine;

public interface IWorkerJob
{
    WorkerJobType JobType { get; }

    /// Найти подходящий ресурс
    ResourceNodeBase FindResource(Vector2 from);

    /// Выдать награду
    void GiveReward(int amount);
}
