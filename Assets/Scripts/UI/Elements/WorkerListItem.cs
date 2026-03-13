using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkerListItem : MonoBehaviour, IPointerClickHandler

{
    [SerializeField] private Text workerText;

    private Worker worker;

    public void Bind(Worker worker)
    {
        this.worker = worker;

        worker.OnJobChanged += UpdateView;
        worker.OnActivityChanged += UpdateView;

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

        string cargoText = worker.HasCargo ? "Да" : "Нет";

        workerText.text =
            $"{worker.name}\n" +
            $"Работа: {currentJob}\n" +
            $"Следующая: {pendingJob}\n" +
            $"Состояние: {GetReadableState(worker.CurrentStateName)}\n" +
            $"Груз: {cargoText}";
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
        GameServices.Instance.Selection?.SelectWorkerFromUI(worker);
    }

}
