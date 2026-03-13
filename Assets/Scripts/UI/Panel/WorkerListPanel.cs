using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerListPanel : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private WorkerListItem itemPrefab;
    [SerializeField] private SelectionSystem selectionSystem;

    private House currentHouse;

    public void Bind(House house)
    {
        currentHouse = house;
        Refresh();
    }

    public void Refresh()
    {
        if (contentRoot == null || itemPrefab == null)
            return;

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        if (currentHouse == null)
            return;

        foreach (Worker worker in currentHouse.Workers)
        {
            if (worker == null)
                continue;

            WorkerListItem item = Instantiate(itemPrefab, contentRoot);
            item.Bind(worker, selectionSystem);
        }
    }
}
