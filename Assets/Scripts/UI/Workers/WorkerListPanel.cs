using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// UI-панель списка рабочих выбранного дома
/// —оздаЄт item дл€ каждого рабочего и позвол€ет выбрать рабочего из списка
/// </summary>
public class WorkerListPanel : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Items")]
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private WorkerListItem _itemPrefab;

    private readonly List<WorkerListItem> _items = new();

    private SelectionSystem _selectionSystem;
    private House _currentHouse;

    [Inject]
    private void Construct(SelectionSystem selectionSystem)
    {
        _selectionSystem = selectionSystem;
    }

    private void Awake()
    {
        Hide();
    }

    /// <summary>
    /// ѕоказывает список рабочих дл€ выбранного дома
    /// </summary>
    public void Show(House house)
    {
        if (house == null)
        {
            Hide();
            return;
        }

        if (_currentHouse == house)
        {
            ShowRoot();
            Rebuild();
            return;
        }

        UnsubscribeFromHouse();

        _currentHouse = house;

        SubscribeToHouse();

        ShowRoot();
        Rebuild();
    }

    /// <summary>
    /// —крывает панель и очищает список
    /// </summary>
    public void Hide()
    {
        UnsubscribeFromHouse();

        _currentHouse = null;

        ClearItems();

        if (_root != null)
            _root.SetActive(false);
    }

    private void Rebuild()
    {
        ClearItems();

        if (_currentHouse == null)
            return;

        IReadOnlyList<Worker> workers = _currentHouse.Workers;
        if (workers == null || workers.Count == 0)
            return;

        foreach (Worker worker in workers)
        {
            if (worker == null)
                continue;

            WorkerListItem item = CreateItem();
            if (item == null)
                continue;

            item.Bind(worker, SelectWorker);
            _items.Add(item);
        }
    }

    private WorkerListItem CreateItem()
    {
        if (_itemPrefab == null || _contentRoot == null)
            return null;

        return Instantiate(_itemPrefab, _contentRoot);
    }

    private void SelectWorker(Worker worker)
    {
        if (worker == null || _selectionSystem == null)
            return;

        _selectionSystem.SelectWorkerFromUI(worker);
    }

    private void SubscribeToHouse()
    {
        if (_currentHouse == null)
            return;

        _currentHouse.OnWorkersChanged += Rebuild;
    }

    private void UnsubscribeFromHouse()
    {
        if (_currentHouse == null)
            return;

        _currentHouse.OnWorkersChanged -= Rebuild;
    }

    private void ClearItems()
    {
        foreach (WorkerListItem item in _items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        _items.Clear();
    }

    private void ShowRoot()
    {
        if (_root != null)
            _root.SetActive(true);
    }
}
