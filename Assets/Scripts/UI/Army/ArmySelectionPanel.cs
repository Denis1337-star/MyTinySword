using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Показывает состав выделенной группы по типам юнитов
/// и кнопку "Выбрать всех"
/// </summary>
public class ArmySelectionPanel : MonoBehaviour
{
    [Header("Items")]
    [FormerlySerializedAs("contentRoot")]
    [SerializeField] private Transform _contentRoot;

    [FormerlySerializedAs("itemPrefab")]
    [SerializeField] private ArmySelectionItem _itemPrefab;

    [Header("Buttons")]
    [FormerlySerializedAs("selectAllButton")]
    [SerializeField] private Button _selectAllButton;

    private readonly List<ArmySelectionItem> _items = new();

    private SelectionSystem _selectionSystem;
    private ArmyUnitRegistry _armyUnitRegistry;
    private bool _isSubscribedToRegistry;

    [Inject]
    private void Construct(
        SelectionSystem selectionSystem,
        ArmyUnitRegistry armyUnitRegistry)
    {
        _selectionSystem = selectionSystem;
        _armyUnitRegistry = armyUnitRegistry;
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_selectAllButton != null)
            _selectAllButton.onClick.AddListener(SelectAllPlayerUnits);

        SubscribeToRegistry();
        RefreshFromCurrentSelection();
    }

    private void OnDisable()
    {
        if (_selectAllButton != null)
            _selectAllButton.onClick.RemoveListener(SelectAllPlayerUnits);

        UnsubscribeFromRegistry();
    }

    /// <summary>
    /// Показывает панель на основе текущего списка выбранных объектов
    /// </summary>
    public void Show(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        ClearItems();

        if (selectedUnits == null || selectedUnits.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        Dictionary<ArmyUnitType, GroupInfo> groups = BuildGroups(selectedUnits);

        if (groups.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        foreach (KeyValuePair<ArmyUnitType, GroupInfo> pair in groups)
        {
            GroupInfo group = pair.Value;

            if (group.Count <= 0)
                continue;

            ArmySelectionItem item = CreateItem();
            if (item == null)
                continue;

            item.Bind(group.Icon, group.Count);
            _items.Add(item);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Скрывает панель и очищает созданные UI-элементы
    /// </summary>
    public void Hide()
    {
        ClearItems();
        gameObject.SetActive(false);
    }

    private ArmySelectionItem CreateItem()
    {
        if (_itemPrefab == null || _contentRoot == null)
            return null;

        return Instantiate(_itemPrefab, _contentRoot);
    }

    private Dictionary<ArmyUnitType, GroupInfo> BuildGroups(IReadOnlyList<UnitSelectable> selectedUnits)
    {
        Dictionary<ArmyUnitType, GroupInfo> result = new();

        foreach (UnitSelectable selectable in selectedUnits)
        {
            if (selectable == null)
                continue;

            ArmyUnit armyUnit = selectable.GetComponent<ArmyUnit>();
            if (armyUnit == null)
                armyUnit = selectable.GetComponentInParent<ArmyUnit>();

            if (armyUnit == null)
                armyUnit = selectable.GetComponentInChildren<ArmyUnit>();

            if (armyUnit == null || !armyUnit.IsPlayerUnit())
                continue;

            UnitConfig config = armyUnit.Config;
            if (config == null)
            {
                Debug.LogWarning($"{armyUnit.name}: ArmyUnit не имеет UnitConfig, поэтому не будет показан в ArmySelectionPanel.", armyUnit);
                continue;
            }

            ArmyUnitType type = config.UnitType;

            if (!result.TryGetValue(type, out GroupInfo group))
            {
                group = new GroupInfo
                {
                    Icon = config.Icon,
                    Count = 0
                };

                result.Add(type, group);
            }

            group.Count++;
            result[type] = group;
        }

        return result;
    }

    private void ClearItems()
    {
        foreach (ArmySelectionItem item in _items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        _items.Clear();
    }

    private void SelectAllPlayerUnits()
    {
        if (_selectionSystem == null || _armyUnitRegistry == null)
            return;

        List<ArmyUnit> units = _armyUnitRegistry.GetAllPlayerUnits();

        _selectionSystem.SelectArmyUnits(units);
    }

    private struct GroupInfo
    {
        public Sprite Icon;
        public int Count;
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
}
