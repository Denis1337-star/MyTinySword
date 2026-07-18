using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

/// <summary>
/// Универсальная панель действий для простых зданий.
/// </summary>
public sealed class BuildingActionPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("UI")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;

    [Header("Demolish")]
    [SerializeField] private Button _demolishButton;

    private BuildingBase _currentBuilding;
    private BuildingDemolishService _buildingDemolishService;

    public SimplePanelTween PanelTween => _panelTween;

    [Inject]
    private void Construct(BuildingDemolishService buildingDemolishService)
    {
        _buildingDemolishService = buildingDemolishService;
    }

    private void OnEnable()
    {
        _demolishButton.onClick.AddListener(RequestDemolishBuilding);
        YG2.onSwitchLang += HandleSwitchLang;
        Refresh();
    }

    private void OnDisable()
    {
        _demolishButton.onClick.RemoveListener(RequestDemolishBuilding);
        YG2.onSwitchLang -= HandleSwitchLang;
    }

    private void HandleSwitchLang(string lang)
    {
        Refresh();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));
        valid &= ValidationUtility.IsAssigned(this, _titleText, nameof(_titleText));
        valid &= ValidationUtility.IsAssigned(this, _descriptionText, nameof(_descriptionText));
        valid &= ValidationUtility.IsAssigned(this, _demolishButton, nameof(_demolishButton));

        return valid;
    }

    public void Show(BuildingBase building)
    {
        if (building == null)
        {
            Hide();
            return;
        }

        _currentBuilding = building;

        Refresh();
    }

    public void Hide()
    {
        _currentBuilding = null;

        ClearView();
    }

    private void Refresh()
    {
        if (_currentBuilding == null)
        {
            ClearView();
            return;
        }

        _titleText.text = _currentBuilding.DisplayName;

        bool canDemolish = BuildingDemolishRules.CanDemolish(_currentBuilding);

        _descriptionText.text = canDemolish
            ? GameUiText.CanDemolishBuilding
            : GameUiText.CannotDemolishBuilding;

        BuildingDemolishRules.RefreshButton(_demolishButton, _currentBuilding);
    }

    private void RequestDemolishBuilding()
    {
        _buildingDemolishService.RequestDemolish(_currentBuilding);
    }

    private void ClearView()
    {
        _titleText.text = string.Empty;
        _descriptionText.text = string.Empty;

        _demolishButton.interactable = false;
        _demolishButton.gameObject.SetActive(false);
    }
}