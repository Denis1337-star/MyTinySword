using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkerListPanel : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private WorkerListItem itemPrefab;
    [SerializeField] private SelectionSystem selectionSystem;

    private House currentHouse;
    private readonly Dictionary<Worker, WorkerListItem> itemsByWorker = new();

    public void Bind(House house)
    {
        if (currentHouse == house)
        {
            Refresh();
            return;
        }

        ClearAllItems();
        currentHouse = house;
        Refresh();
    }

    public void Refresh()
    {
        if (contentRoot == null || itemPrefab == null)
            return;

        if (currentHouse == null)
        {
            ClearAllItems();
            return;
        }

        RemoveMissingWorkers();
        AddMissingWorkers();
        CleanupNullEntries();
    }

    private void RemoveMissingWorkers()
    {
        List<Worker> toRemove = new();

        foreach (var pair in itemsByWorker)
        {
            Worker worker = pair.Key;

            if (worker == null || currentHouse.Workers == null || !currentHouse.Workers.Contains(worker))
                toRemove.Add(worker);
        }

        foreach (Worker worker in toRemove)
        {
            RemoveItem(worker);
        }
    }

    private void AddMissingWorkers()
    {
        if (currentHouse.Workers == null)
            return;

        foreach (Worker worker in currentHouse.Workers)
        {
            if (worker == null)
                continue;

            if (itemsByWorker.ContainsKey(worker))
                continue;

            WorkerListItem item = Instantiate(itemPrefab, contentRoot);
            item.Bind(worker, selectionSystem);
            itemsByWorker.Add(worker, item);
        }
    }

    private void RemoveItem(Worker worker)
    {
        if (worker == null)
            return;

        if (!itemsByWorker.TryGetValue(worker, out WorkerListItem item))
            return;

        if (item != null)
            Destroy(item.gameObject);

        itemsByWorker.Remove(worker);
    }

    private void CleanupNullEntries()
    {
        List<Worker> invalidWorkers = new();

        foreach (var pair in itemsByWorker)
        {
            if (pair.Key == null || pair.Value == null)
                invalidWorkers.Add(pair.Key);
        }

        foreach (Worker worker in invalidWorkers)
        {
            RemoveItem(worker);
        }
    }

    private void ClearAllItems()
    {
        foreach (var pair in itemsByWorker)
        {
            if (pair.Value != null)
                Destroy(pair.Value.gameObject);
        }

        itemsByWorker.Clear();
    }
}
