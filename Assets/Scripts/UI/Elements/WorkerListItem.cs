using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerListItem : MonoBehaviour
{
    [SerializeField] private Text workerText;

    private Worker worker;

    public void Bind(Worker worker)
    {
        this.worker = worker;

        worker.OnStateChanged += UpdateView;
        worker.OnJobChanged += UpdateView;

        UpdateView(worker);
    }

    private void UpdateView(Worker w)
    {
        workerText.text =
            $"{w.name}\n" +
            $"Работа: {WorkerJobLocalization.GetName(w.CurrentJob)}";
    }

    private void OnDestroy()
    {
        if (worker == null) return;

        worker.OnStateChanged -= UpdateView;
        worker.OnJobChanged -= UpdateView;
    }

}
