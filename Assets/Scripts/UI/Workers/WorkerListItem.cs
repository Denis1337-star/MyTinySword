using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI-элемент одного worker'а в списке дома
/// Показывает имя, текущую работу, pending job и текущее состояние,
/// а также позволяет выбрать worker'а по клику из UI
/// </summary>
public class WorkerListItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text workerText;

    private Worker worker;
    private SelectionSystem selectionSystem;

    /// <summary>
    /// Привязывает item к конкретному worker'у и selection system.
    /// </summary>
    public void Bind(Worker worker, SelectionSystem selectionSystem)
    {
        Unsubscribe();

        this.worker = worker;
        this.selectionSystem = selectionSystem;

        Subscribe();
        UpdateView();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    /// <summary>
    /// Подписывается на события worker'а,
    /// чтобы строка автоматически обновлялась при изменениях.
    /// </summary>
    private void Subscribe()
    {
        if (worker == null)
            return;

        worker.OnJobChanged += UpdateView;
        worker.OnActivityChanged += UpdateView;
    }

    /// <summary>
    /// Снимает подписки с текущего worker'а.
    /// </summary>
    private void Unsubscribe()
    {
        if (worker == null)
            return;

        worker.OnJobChanged -= UpdateView;
        worker.OnActivityChanged -= UpdateView;
    }

    /// <summary>
    /// Обновляет текст строки под текущее состояние worker'а.
    /// </summary>
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

    /// <summary>
    /// Переводит внутреннее имя состояния worker'а в понятный текст для UI.
    /// </summary>
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

    /// <summary>
    /// По клику на строку выбираем соответствующего worker'а через общую selection-систему.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (worker == null || selectionSystem == null)
            return;

        selectionSystem.SelectWorkerFromUI(worker);
    }
}