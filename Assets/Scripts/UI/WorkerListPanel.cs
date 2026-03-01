using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerListPanel : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private WorkerListItem itemPrefab;

    private void Start()
    {
        foreach (var worker in WorkerRegistry.Instance.Workers)
        {
            CreateItem(worker);
        }
    }

    private void CreateItem(Worker worker)
    {
        var item = Instantiate(itemPrefab, contentRoot);
        item.Bind(worker);
    }
}
