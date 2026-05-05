using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-элемент одного рабочего в списке рабочих дома
/// Показывает работу, состояние и позволяет выбрать рабочего
/// </summary>
public class WorkerListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private Button _selectButton;

    private Worker _worker;
    private Action<Worker> _onSelected;

    private void OnEnable()
    {
        _selectButton?.onClick.AddListener(HandleSelectClicked);

        SubscribeToWorker();
    }

    private void OnDisable()
    {
        _selectButton?.onClick.RemoveListener(HandleSelectClicked);

        UnsubscribeFromWorker();
    }

    /// <summary>
    /// Привязывает item к конкретному рабочему
    /// </summary>
    public void Bind(Worker worker, Action<Worker> onSelected)
    {
        UnsubscribeFromWorker();

        _worker = worker;
        _onSelected = onSelected;

        SubscribeToWorker();
        Refresh();
    }

    private void Refresh()
    {
        if (_infoText == null)
            return;

        if (_worker == null)
        {
            _infoText.text = string.Empty;
            return;
        }

        string currentJob = WorkerJobLocalization.GetName(_worker.CurrentJob);

        string jobLine = _worker.HasPendingJob
            ? $"Job: {currentJob} → {WorkerJobLocalization.GetName(_worker.PendingJob)}"
            : $"Job: {currentJob}";

        _infoText.text =
            $"{_worker.name}\n" +
            $"{jobLine}";
    }

    private void HandleSelectClicked()
    {
        if (_worker == null)
            return;

        _onSelected?.Invoke(_worker);
    }

    private void SubscribeToWorker()
    {
        if (_worker == null)
            return;

        _worker.OnJobChanged += Refresh;
        _worker.OnActivityChanged += Refresh;
    }

    private void UnsubscribeFromWorker()
    {
        if (_worker == null)
            return;

        _worker.OnJobChanged -= Refresh;
        _worker.OnActivityChanged -= Refresh;
    }
}