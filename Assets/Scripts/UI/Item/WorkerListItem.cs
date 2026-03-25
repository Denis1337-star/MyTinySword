using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkerListItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text workerText;

    private Worker worker;
    private SelectionSystem selectionSystem;

    public void Bind(Worker worker, SelectionSystem selectionSystem)
    {
        Unsubscribe();

        this.worker = worker;
        this.selectionSystem = selectionSystem;

        Subscribe();
        UpdateView();
    }

    private void Subscribe()
    {
        if (worker == null)
            return;

        worker.OnJobChanged += UpdateView;
        worker.OnActivityChanged += UpdateView;
    }

    private void Unsubscribe()
    {
        if (worker == null)
            return;

        worker.OnJobChanged -= UpdateView;
        worker.OnActivityChanged -= UpdateView;
    }

    private void UpdateView()
    {
        if (worker == null || workerText == null)
            return;

        string currentJob = WorkerJobLocalization.GetName(worker.CurrentJob);
        string pendingJob = worker.HasPendingJob
            ? WorkerJobLocalization.GetName(worker.PendingJob)
            : "Нет";

        workerText.text =
            $"{worker.name}\n" +
            $"Работа: {currentJob}\n" +
            $"Следующая: {pendingJob}\n" +
            $"Состояние: {GetReadableState(worker.CurrentStateName)}\n";
    }

    private string GetReadableState(string stateName)
    {
        return stateName switch
        {
            nameof(WorkerIdleState) => "Ожидает",
            nameof(WorkerFindResourceState) => "Ищет ресурс",
            nameof(WorkerGoToResourceState) => "Идёт к ресурсу",
            nameof(WorkerWorkState) => "Работает",
            nameof(WorkerCarryState) => "Несёт ресурс",
            _ => stateName
        };
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (worker == null || selectionSystem == null)
            return;

        selectionSystem.SelectWorkerFromUI(worker);
    }
}