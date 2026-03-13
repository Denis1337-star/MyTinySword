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

        UnsubscribeFromHouse();
        ClearAllItems();

        currentHouse = house;

        SubscribeToHouse();
        Refresh();
    }

    private void OnDisable()
    {
        UnsubscribeFromHouse();
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

    private void SubscribeToHouse()
    {
        if (currentHouse == null)
            return;

        currentHouse.OnWorkerAdded += OnWorkerAdded;
        currentHouse.OnWorkerRemoved += OnWorkerRemoved;
        currentHouse.OnWorkersChanged += OnWorkersChanged;
    }

    private void UnsubscribeFromHouse()
    {
        if (currentHouse == null)
            return;

        currentHouse.OnWorkerAdded -= OnWorkerAdded;
        currentHouse.OnWorkerRemoved -= OnWorkerRemoved;
        currentHouse.OnWorkersChanged -= OnWorkersChanged;
    }

    private void OnWorkerAdded(Worker worker)
    {
        if (worker == null)
            return;

        AddWorkerItem(worker);
    }

    private void OnWorkerRemoved(Worker worker)
    {
        if (worker == null)
            return;

        RemoveItem(worker);
    }

    private void OnWorkersChanged()
    {
        Refresh();
    }

    private void RemoveMissingWorkers()
    {
        List<Worker> toRemove = new();

        foreach (var pair in itemsByWorker)
        {
            Worker worker = pair.Key;

            if (worker == null || currentHouse.Workers == null || !ContainsWorker(currentHouse.Workers, worker))
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
            AddWorkerItem(worker);
        }
    }

    private void AddWorkerItem(Worker worker)
    {
        if (worker == null || itemPrefab == null || contentRoot == null)
            return;

        if (itemsByWorker.ContainsKey(worker))
            return;

        WorkerListItem item = Instantiate(itemPrefab, contentRoot);
        item.Bind(worker, selectionSystem);
        itemsByWorker.Add(worker, item);
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

    private bool ContainsWorker(IReadOnlyList<Worker> workers, Worker target)
    {
        if (workers == null || target == null)
            return false;

        for (int i = 0; i < workers.Count; i++)
        {
            if (workers[i] == target)
                return true;
        }

        return false;
    }
}
