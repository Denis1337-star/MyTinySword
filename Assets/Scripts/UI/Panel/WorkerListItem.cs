using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI элемент одного рабочего в списке рабочих дома
/// </summary>
public sealed class WorkerListItem : ValidatedMonoBehaviour
{
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private Button _selectButton;

    private Worker _worker;
    private Worker _subscribedWorker;
    private Action<Worker> _onSelected;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _infoText, nameof(_infoText));
        valid &= ValidationUtility.IsAssigned(this, _selectButton, nameof(_selectButton));

        return valid;
    }

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(HandleSelectClicked);

        SubscribeToWorker();
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(HandleSelectClicked);

        UnsubscribeFromWorker();
    }

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
        if (_worker == null)
        {
            _infoText.text = string.Empty;
            return;
        }

        string currentJob = WorkerJobLocalization.GetName(_worker.CurrentJob);

        string jobLine = _worker.HasPendingJob
            ? $"Работа: {currentJob} → {WorkerJobLocalization.GetName(_worker.PendingJob)}"
            : $"Работа: {currentJob}";

        _infoText.text =
            $"{_worker.name}\n" +
            $"{jobLine}";
    }

    private void HandleSelectClicked()
    {
        _onSelected?.Invoke(_worker);
    }

    private void SubscribeToWorker()
    {
        if (_worker == null)
            return;

        if (_subscribedWorker == _worker)
            return;

        _worker.OnJobChanged += Refresh;

        _subscribedWorker = _worker;
    }

    private void UnsubscribeFromWorker()
    {
        if (_subscribedWorker == null)
            return;

        _subscribedWorker.OnJobChanged -= Refresh;
        _subscribedWorker = null;
    }
}