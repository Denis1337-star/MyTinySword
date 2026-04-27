using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI-панель списка worker текущего дома
/// </summary>
public class WorkerListPanel : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private WorkerListItem itemPrefab;
    [SerializeField] private SelectionSystem selectionSystem;

    // Дом, список worker'ов которого сейчас отображается
    private House currentHouse;

    // Связь между gameplay-worker'ом и его UI-элементом
    private readonly Dictionary<Worker, WorkerListItem> itemsByWorker = new();

    /// <summary>
    /// Привязывает панель к указанному дому.
    /// </summary>
    public void Bind(House house)
    {
        // Если дом тот же самый — просто обновляем содержимое
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

    private void OnDestroy()
    {
        UnsubscribeFromHouse();
        ClearAllItems();
    }

    /// <summary>
    /// Полностью синхронизирует UI-список с текущим составом worker'ов дома.
    /// </summary>
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

    /// <summary>
    /// Подписывается на события текущего дома.
    /// </summary>
    private void SubscribeToHouse()
    {
        if (currentHouse == null)
            return;

        currentHouse.OnWorkerAdded += OnWorkerAdded;
        currentHouse.OnWorkerRemoved += OnWorkerRemoved;
        currentHouse.OnWorkersChanged += OnWorkersChanged;
    }

    /// <summary>
    /// Снимает подписки с текущего дома.
    /// </summary>
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

    /// <summary>
    /// Удаляет UI-элементы для worker'ов, которые больше не принадлежат текущему дому.
    /// </summary>
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
            RemoveItem(worker);
    }

    /// <summary>
    /// Добавляет недостающие UI-элементы для всех worker'ов текущего дома.
    /// </summary>
    private void AddMissingWorkers()
    {
        if (currentHouse.Workers == null)
            return;

        foreach (Worker worker in currentHouse.Workers)
            AddWorkerItem(worker);
    }

    /// <summary>
    /// Создаёт UI-элемент для worker'а, если его ещё нет в списке.
    /// </summary>
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

    /// <summary>
    /// Удаляет UI-элемент указанного worker'а.
    /// </summary>
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

    /// <summary>
    /// Очищает битые записи, если worker или item уже были уничтожены.
    /// </summary>
    private void CleanupNullEntries()
    {
        List<Worker> invalidWorkers = new();

        foreach (var pair in itemsByWorker)
        {
            if (pair.Key == null || pair.Value == null)
                invalidWorkers.Add(pair.Key);
        }

        foreach (Worker worker in invalidWorkers)
            RemoveItem(worker);
    }

    /// <summary>
    /// Полностью удаляет все UI-элементы списка.
    /// </summary>
    private void ClearAllItems()
    {
        foreach (var pair in itemsByWorker)
        {
            if (pair.Value != null)
                Destroy(pair.Value.gameObject);
        }

        itemsByWorker.Clear();
    }

    /// <summary>
    /// Проверяет, содержится ли worker в списке worker'ов дома.
    /// </summary>
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
