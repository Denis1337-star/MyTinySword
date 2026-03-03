using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerListPanel : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private WorkerListItem itemPrefab;
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
