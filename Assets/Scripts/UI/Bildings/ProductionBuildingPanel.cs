using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Общая UI-панель производственных зданий
/// </summary>
public class ProductionBuildingPanel : MonoBehaviour
{
    [Header("Root")]
    [FormerlySerializedAs("root")]
    [SerializeField] private GameObject _root;

    [Header("Text")]
    [FormerlySerializedAs("titleText")]
    [SerializeField] private TMP_Text _titleText;

    [FormerlySerializedAs("costText")]
    [SerializeField] private TMP_Text _costText;

    [FormerlySerializedAs("blockReasonText")]
    [SerializeField] private TMP_Text _blockReasonText;

    [Header("Button")]
    [FormerlySerializedAs("hireButton")]
    [SerializeField] private Button _hireButton;

    private ProductionBuildingBase _currentBuilding;

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        if (_hireButton != null)
            _hireButton.onClick.AddListener(HireUnit);
    }

    private void OnDisable()
    {
        if (_hireButton != null)
            _hireButton.onClick.RemoveListener(HireUnit);
    }

    public void Show(ProductionBuildingBase building)
    {
        if (building == null)
        {
            Hide();
            return;
        }

        _currentBuilding = building;

        if (_root != null)
            _root.SetActive(true);

        Refresh();
    }

    public void Hide()
    {
        _currentBuilding = null;

        if (_root != null)
            _root.SetActive(false);
    }

    private void HireUnit()
    {
        if (_currentBuilding == null)
            return;

        _currentBuilding.TryHireUnit();
        Refresh();
    }

    private void Refresh()
    {
        if (_currentBuilding == null)
            return;

        if (_titleText != null)
            _titleText.text = _currentBuilding.DisplayName;

        if (_costText != null)
        {
            _costText.text =
               $"Wood: {_currentBuilding.WoodCost}  Meat: {_currentBuilding.MeatCost}";
        }

        string blockReason = _currentBuilding.GetHireBlockReason();

        if (_blockReasonText != null)
            _blockReasonText.text = blockReason;

        if (_hireButton != null)
            _hireButton.interactable = string.IsNullOrEmpty(blockReason);
    }
}