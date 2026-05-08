using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// UI панель списка рабочих выбранного дома
/// </summary>
public sealed class WorkerListPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Items")]
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private WorkerListItem _itemPrefab;

    private readonly List<WorkerListItem> _items = new();

    private SelectionSystem _selectionSystem;
    private House _currentHouse;
    private House _subscribedHouse;

    [Inject]
    private void Construct(SelectionSystem selectionSystem)
    {
        _selectionSystem = selectionSystem;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        Hide();
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

    public void Hide()
    {
        UnsubscribeFromHouse();

        _currentHouse = null;

        ClearItems();
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
        if (_itemPrefab == null || _contentRoot == null)
            return null;

        return Instantiate(_itemPrefab, _contentRoot);
    }

    private void SelectWorker(Worker worker)
    {
        if (worker == null)
            return;

        _selectionSystem.SelectWorkerFromUI(worker);
    }

    private void SubscribeToHouse()
    {
        if (_currentHouse == null)
            return;

        if (_subscribedHouse == _currentHouse)
            return;

        _currentHouse.OnWorkersChanged += Rebuild;
        _subscribedHouse = _currentHouse;
    }

    private void UnsubscribeFromHouse()
    {
        if (_subscribedHouse == null)
            return;

        _subscribedHouse.OnWorkersChanged -= Rebuild;
        _subscribedHouse = null;
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