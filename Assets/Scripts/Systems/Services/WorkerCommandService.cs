/// <summary>
/// Сервис команд для worker.
/// Отвечает только за назначение новой работы.
/// Не перезапускает текущую работу повторным нажатием,
/// потому что кнопка текущей работы должна быть disabled в UI.
/// </summary>
public sealed class WorkerCommandService
{
    public bool TryAssignJob(Worker worker, WorkerJobType job)
    {
        if (worker == null)
            return false;

        if (worker.CurrentJob == job && !worker.HasPendingJob)
            return false;

        if (worker.PendingJob == job)
            return false;

        worker.AssignJob(job);
        return true;
    }
}