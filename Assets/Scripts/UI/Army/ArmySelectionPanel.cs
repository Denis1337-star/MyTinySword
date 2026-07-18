using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Показывает состав выделенной группы боевых юнитов по типам
/// и кнопку Выбрать всех
/// </summary>
public sealed class ArmySelectionPanel : ValidatedMonoBehaviour
{
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private ArmySelectionItem _itemPrefab;
    [SerializeField] private Button _selectAllButton;
    [SerializeField] private SimplePanelTween _panelTween;

    private readonly List<ArmySelectionItem> _items = new();
    private readonly Dictionary<ArmyUnitType, GroupInfo> _groups = new();
    private readonly List<ArmyUnit> _playerUnitsBuffer = new();

    private SelectionSystem _selectionSystem;
    private ArmyUnitRegistry _armyUnitRegistry;

    private bool _isApplyingShow;

    public SimplePanelTween PanelTween => _panelTween;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        ArmyUnitRegistry armyUnitRegistry)
    {
        _selectionSystem = selectionSystem;
        _armyUnitRegistry = armyUnitRegistry;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        _selectAllButton.onClick.AddListener(SelectAllPlayerUnits);
    }

    private void OnDisable()
    {
        _selectAllButton.onClick.RemoveListener(SelectAllPlayerUnits);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _contentRoot, nameof(_contentRoot));
        valid &= ValidationUtility.IsAssigned(this, _itemPrefab, nameof(_itemPrefab));
        valid &= ValidationUtility.IsAssigned(this, _selectAllButton, nameof(_selectAllButton));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));

        return valid;
    }

    public bool Show(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        _isApplyingShow = true;

        ClearItems();
        BuildGroups(selectedUnits);

        if (_groups.Count == 0)
        {
            _isApplyingShow = false;
            return false;
        }

        foreach (KeyValuePair<ArmyUnitType, GroupInfo> pair in _groups)
        {
            GroupInfo group = pair.Value;

            if (group.Count <= 0)
                continue;

            ArmySelectionItem item = CreateItem();
            item.Bind(group.Icon, group.Count);

            _items.Add(item);
        }

        _isApplyingShow = false;
        return true;
    }

    public void Hide()
    {
        ClearItems();
        _groups.Clear();
    }

    private ArmySelectionItem CreateItem()
    {
        return Instantiate(_itemPrefab, _contentRoot);
    }

    private void BuildGroups(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        _groups.Clear();

        if (selectedUnits == null || selectedUnits.Count == 0)
            return;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            UnitSelectable selectable = selectedUnits[i];

            if (selectable == null)
                continue;

            if (!ArmyUnitSelectionUtility.TryGetPlayerArmyUnit(selectable, out ArmyUnit armyUnit))
                continue;

            UnitConfig config = armyUnit.Config;

            if (config == null)
            {
                Debug.LogWarning(
                    $"{armyUnit.name}: ArmyUnit не имеет UnitConfig",
                    armyUnit);

                continue;
            }

            ArmyUnitType type = config.UnitType;

            if (!_groups.TryGetValue(type, out GroupInfo group))
            {
                group = new GroupInfo
                {
                    Icon = config.Icon,
                    Count = 0
                };
            }

            group.Count++;
            _groups[type] = group;
        }
    }

    private void ClearItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            ArmySelectionItem item = _items[i];

            if (item != null)
                Destroy(item.gameObject);
        }

        _items.Clear();
    }

    private void SelectAllPlayerUnits()
    {
        _armyUnitRegistry.GetAllPlayerUnitsNonAlloc(_playerUnitsBuffer);

        _selectionSystem.SelectArmyUnits(_playerUnitsBuffer);
    }

    private struct GroupInfo
    {
        public Sprite Icon;
        public int Count;
    }
}