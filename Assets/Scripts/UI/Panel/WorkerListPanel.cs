using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerListPanel : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private WorkerListItem itemPrefab;

    private void Awake()
    {
        WorkerRegistry.Instance.OnWorkerAdded += CreateItem;
    }

    private void OnDestroy()
    {
        if (WorkerRegistry.Instance != null)
            WorkerRegistry.Instance.OnWorkerAdded -= CreateItem;
    }

    private void CreateItem(Worker worker)
    {
        var item = Instantiate(itemPrefab, contentRoot);
        item.Bind(worker);
    }
    public void Refresh()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var worker in WorkerRegistry.Instance.Workers)
        {
            var item = Instantiate(itemPrefab, contentRoot);
            item.Bind(worker);
        }
    }
}
