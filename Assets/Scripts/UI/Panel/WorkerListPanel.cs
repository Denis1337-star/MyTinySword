using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// UI-панель списка рабочих выбранного дома.
/// Список обновляется через HousePanel — без отдельной подписки на дом.
/// </summary>
public sealed class WorkerListPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Items")]
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private WorkerListItem _itemPrefab;

    private readonly List<WorkerListItem> _items = new();

    private DiContainer _container;
    private SelectionSystem _selectionSystem;
    private House _currentHouse;

    [Inject]
    private void Construct(
        DiContainer container,
        SelectionSystem selectionSystem)
    {
        _container = container;
        _selectionSystem = selectionSystem;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _contentRoot, nameof(_contentRoot));
        valid &= ValidationUtility.IsAssigned(this, _itemPrefab, nameof(_itemPrefab));

        return valid;
    }

    public void Show(House house)
    {
        if (house == null)
        {
            Hide();
            return;
        }

        _currentHouse = house;
        ShowRoot();
        Rebuild();
    }

    public void Hide()
    {
        _currentHouse = null;
        ClearItems();
        _root.SetActive(false);
    }

    public void Rebuild()
    {
        ClearItems();

        if (_currentHouse == null)
            return;

        IReadOnlyList<Worker> workers = _currentHouse.Workers;

        if (workers == null || workers.Count == 0)
            return;

        for (int i = 0; i < workers.Count; i++)
        {
            Worker worker = workers[i];

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
        return _container.InstantiatePrefabForComponent<WorkerListItem>(
            _itemPrefab,
            _contentRoot);
    }

    private void SelectWorker(Worker worker)
    {
        if (worker == null)
            return;

        _selectionSystem.SelectWorkerFromUI(worker);
    }

    private void ClearItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            WorkerListItem item = _items[i];

            if (item != null)
                Destroy(item.gameObject);
        }

        _items.Clear();
    }

    private void ShowRoot()
    {
        _root.SetActive(true);
    }
}
