using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerIdleState : IWorkerState
{
    private readonly Worker worker;

    public WorkerIdleState(Worker worker)
    {
        this.worker = worker;
    }

    public void Enter()
    {
        worker.Animator.SetWorking(false);
        worker.Animator.SetEquipment(EquipmentType.None);

        worker.Movement.MoveTo(
            worker.Home.GetIdlePosition(worker)
        );
    }

    public void Update()
    {
        // ничего не делаем
    }

    public void Exit()
    {
    }
}
