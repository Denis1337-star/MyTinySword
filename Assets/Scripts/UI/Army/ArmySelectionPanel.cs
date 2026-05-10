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

    private readonly List<ArmySelectionItem> _items = new();
    private readonly Dictionary<ArmyUnitType, GroupInfo> _groups = new();
    private readonly List<ArmyUnit> _playerUnitsBuffer = new();

    private SelectionSystem _selectionSystem;
    private ArmyUnitRegistry _armyUnitRegistry;

    private bool _isSubscribedToRegistry;
    private bool _isApplyingShow;

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

        if (!enabled)
            return;

        SetPanelActive(false);
    }

    private void OnEnable()
    {
        _selectAllButton.onClick.AddListener(SelectAllPlayerUnits);

        SubscribeToRegistry();

        if (!_isApplyingShow)
            RefreshFromCurrentSelection();
    }

    private void OnDisable()
    {
        _selectAllButton.onClick.RemoveListener(SelectAllPlayerUnits);

        UnsubscribeFromRegistry();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _contentRoot, nameof(_contentRoot));
        valid &= ValidationUtility.IsAssigned(this, _itemPrefab, nameof(_itemPrefab));
        valid &= ValidationUtility.IsAssigned(this, _selectAllButton, nameof(_selectAllButton));

        return valid;
    }

    public void Show(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        _isApplyingShow = true;

        ClearItems();
        BuildGroups(selectedUnits);

        if (_groups.Count == 0)
        {
            SetPanelActive(false);
            _isApplyingShow = false;
            return;
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

        SetPanelActive(true);

        _isApplyingShow = false;
    }

    public void Hide()
    {
        ClearItems();
        _groups.Clear();

        SetPanelActive(false);
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

            ArmyUnit armyUnit = FindArmyUnitNearSelectable(selectable);

            if (armyUnit == null || !armyUnit.IsPlayerUnit())
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

    private ArmyUnit FindArmyUnitNearSelectable(UnitSelectable selectable)
    {
        if (selectable == null)
            return null;

        ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();

        if (armyUnit != null)
            return armyUnit;

        armyUnit = selectable.GetComponentInParent<ArmyUnit>();

        if (armyUnit != null)
            return armyUnit;

        return selectable.GetComponentInChildren<ArmyUnit>();
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
        if (_selectionSystem == null || _armyUnitRegistry == null)
            return;

        _armyUnitRegistry.GetAllPlayerUnitsNonAlloc(_playerUnitsBuffer);

        _selectionSystem.SelectArmyUnits(_playerUnitsBuffer);
    }

    private void SubscribeToRegistry()
    {
        if (_isSubscribedToRegistry)
            return;

        if (_armyUnitRegistry == null)
            return;

        _armyUnitRegistry.OnArmyChanged += RefreshFromCurrentSelection;
        _isSubscribedToRegistry = true;
    }

    private void UnsubscribeFromRegistry()
    {
        if (!_isSubscribedToRegistry)
            return;

        if (_armyUnitRegistry != null)
            _armyUnitRegistry.OnArmyChanged -= RefreshFromCurrentSelection;

        _isSubscribedToRegistry = false;
    }

    private void RefreshFromCurrentSelection()
    {
        if (_selectionSystem == null)
            return;

        Show(_selectionSystem.SelectedUnits);
    }

    private void SetPanelActive(bool isActive)
    {
        if (gameObject.activeSelf == isActive)
            return;

        gameObject.SetActive(isActive);
    }

    private struct GroupInfo
    {
        public Sprite Icon;
        public int Count;
    }
}