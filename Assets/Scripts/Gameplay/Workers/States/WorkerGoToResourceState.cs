using UnityEngine;

/// <summary>
/// —осто€ние движени€ рабочего к заранее назначенному ресурсу и зарезервированному рабочему слоту
/// </summary>
public class WorkerGoToResourceState : IWorkerState
{
    private readonly Worker worker;
    private float repathTimer;
    private const float RepathInterval = 0.4f;

    public WorkerGoToResourceState(Worker worker)
    {
        this.worker = worker;
    }

    /// <summary>
    /// ¬ызываетс€ при входе в состо€ние
    /// ѕровер€ет актуальность назначени€ и запускает движение к рабочему слоту
    /// </summary>
    public void Enter()
    {
        repathTimer = RepathInterval;

        if (!worker.HasValidResourceAssignment())
        {
            RestartResourceSearch();
            return;
        }

        worker.Movement.MoveTo(worker.TargetSlot.Position);
    }

    /// <summary>
    /// ¬ызываетс€ каждый кадр, пока рабочий находитс€ в состо€нии движени€ к ресурсу
    /// </summary>
    public void Update()
    {
        if (!worker.HasValidResourceAssignment())
        {
            RestartResourceSearch();
            return;
        }

        float distance = Vector2.Distance(worker.transform.position, worker.TargetSlot.Position);

        if (distance <= worker.GetReachResourceDistance())
        {
            worker.Movement.Stop();
            worker.EnterWorkState();
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = RepathInterval;
            worker.Movement.MoveTo(worker.TargetSlot.Position);
        }

        if (!worker.Movement.HasTarget)
        {
            RestartResourceSearch();
        }
    }
    private void RestartResourceSearch()
    {
        worker.ClearCurrentAssignment();
        worker.StartFindingResource();
    }

    public void Exit()
    {
    }
}
