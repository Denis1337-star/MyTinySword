using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Нижняя панель выбранной армии.
/// Показывает состав выделенной группы по типам юнитов
/// и кнопку "Выбрать всех".
/// </summary>
public class ArmySelectionPanel : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private ArmySelectionItem itemPrefab;

    [Header("Buttons")]
    [SerializeField] private Button selectAllButton;

    private readonly List<ArmySelectionItem> items = new();

    private void Awake()
    {
        if (selectAllButton != null)
            selectAllButton.onClick.AddListener(OnSelectAllClicked);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (selectAllButton != null)
            selectAllButton.onClick.RemoveListener(OnSelectAllClicked);
    }

    /// <summary>
    /// Показывает панель на основе текущего списка выбранных объектов.
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

        foreach (var pair in groups)
        {
            GroupInfo group = pair.Value;

            if (group.Count <= 0)
                continue;

            ArmySelectionItem item = Instantiate(itemPrefab, contentRoot);
            item.Bind(group.Icon, group.Count);
            items.Add(item);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Скрывает панель.
    /// </summary>
    public void Hide()
    {
        ClearItems();
        gameObject.SetActive(false);
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

            if (armyUnit == null || !armyUnit.IsPlayerUnit())
                continue;

            UnitConfig config = armyUnit.Config;
            if (config == null)
                continue;

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
        foreach (ArmySelectionItem item in items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        items.Clear();
    }

    private void OnSelectAllClicked()
    {
       // SelectAllArmyUnitsButton.Instance?.SelectAllPlayerUnits();
    }

    private struct GroupInfo
    {
        public Sprite Icon;
        public int Count;
    }
}
