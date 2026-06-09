using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Панель выбора уровней.
/// Создаёт элементы уровней из LevelCatalog и запускает выбранный уровень.
/// </summary>
public sealed class LevelSelectPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("Levels")]
    [SerializeField] private LevelCatalog _levelCatalog;
    [SerializeField] private LevelSelectItem _itemPrefab;
    [SerializeField] private Transform _itemsRoot;

    private readonly List<LevelSelectItem> _spawnedItems = new();

    private LevelProgressService _levelProgressService;
    private LevelLoaderService _levelLoaderService;

    [Inject]
    private void Construct(
        LevelProgressService levelProgressService,
        LevelLoaderService levelLoaderService)
    {
        _levelProgressService = levelProgressService;
        _levelLoaderService = levelLoaderService;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));
        valid &= ValidationUtility.IsAssigned(this, _levelCatalog, nameof(_levelCatalog));
        valid &= ValidationUtility.IsAssigned(this, _itemPrefab, nameof(_itemPrefab));
        valid &= ValidationUtility.IsAssigned(this, _itemsRoot, nameof(_itemsRoot));

        if (_levelCatalog != null)
            valid &= _levelCatalog.IsValid();

        return valid;
    }

    private void OnEnable()
    {
        Rebuild();
    }

    private void OnDisable()
    {
        ClearItems();
    }

    public void Show()
    {
        Rebuild();
        _panelTween.Show();
    }

    public void Hide()
    {
        _panelTween.Hide();
    }

    public void Toggle()
    {
        if (_panelTween.IsVisible)
        {
            Hide();
            return;
        }

        Show();
    }

    public void Rebuild()
    {
        ClearItems();

        if (_levelCatalog == null)
            return;

        for (int i = 0; i < _levelCatalog.Levels.Count; i++)
        {
            LevelConfig level = _levelCatalog.Levels[i];

            if (level == null)
                continue;

            CreateItem(level);
        }
    }

    private void CreateItem(LevelConfig levelConfig)
    {
        LevelSelectItem item = Instantiate(_itemPrefab, _itemsRoot);

        bool unlocked = _levelProgressService.IsLevelUnlocked(levelConfig.LevelIndex);
        bool completed = _levelProgressService.IsLevelCompleted(levelConfig.LevelId);

        item.Initialize(
            levelConfig,
            unlocked,
            completed,
            OnLevelClicked);

        _spawnedItems.Add(item);
    }

    private void OnLevelClicked(LevelConfig levelConfig)
    {
        _levelLoaderService.TryLoadLevel(levelConfig);
    }

    private void ClearItems()
    {
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            LevelSelectItem item = _spawnedItems[i];

            if (item != null)
                Destroy(item.gameObject);
        }

        _spawnedItems.Clear();
    }
}