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
        this.worker = worker;
        this.selectionSystem = selectionSystem;

        if (worker != null)
        {
            worker.OnJobChanged += UpdateView;
            worker.OnActivityChanged += UpdateView;
        }

        UpdateView();
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
            nameof(WorkerUnloadState) => "Сдаёт ресурс",
            _ => stateName
        };
    }

    private void OnDestroy()
    {
        if (worker == null)
            return;

        worker.OnJobChanged -= UpdateView;
        worker.OnActivityChanged -= UpdateView;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (worker == null || selectionSystem == null)
            return;

        selectionSystem.SelectWorkerFromUI(worker);
    }
}