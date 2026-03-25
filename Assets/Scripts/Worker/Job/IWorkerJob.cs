using UnityEngine;

public interface IWorkerJob
{
    WorkerJobType JobType { get; }
    ResourceType RewardType { get; }

    ResourceNodeBase FindResource(Vector2 from);
}
