/// <summary>
/// Состояние работы с ресурсом
/// </summary>
public sealed class WorkerWorkState : IWorkerState
{
    private readonly Worker _worker;

    public WorkerWorkState(Worker worker)
    {
        _worker = worker;
    }

    public void Enter()
    {
        if (!_worker.HasValidResourceAssignmentForWork())
        {
            _worker.ClearCurrentAssignment();
            _worker.StateMachine.ChangeState(WorkerStateType.Idle);
            return;
        }

        _worker.Animator.SetWorking(true);

        bool started = _worker.TargetResource.TryStartWork(
            _worker,
            OnWorkFinished);

        if (!started)
        {
            _worker.Animator.SetWorking(false);
            _worker.ClearCurrentAssignment();
            _worker.StateMachine.ChangeState(WorkerStateType.FindResource);
        }
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _worker.Animator.SetWorking(false);
    }

    private void OnWorkFinished(int amount)
    {
        if (_worker.CurrentJobLogic == null)
        {
            _worker.ClearCurrentAssignment();
            _worker.StateMachine.ChangeState(WorkerStateType.Idle);
            return;
        }

        ResourceType resourceType = _worker.CurrentJobLogic.RewardType;

        _worker.Inventory.SetCargo(resourceType, amount);
        _worker.CompleteCurrentAssignment();
        _worker.StateMachine.ChangeState(WorkerStateType.Carry);
    }
}