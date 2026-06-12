using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// ”ниверсальна€ панель действий дл€ простых зданий.
/// Ќапример: Tower, Castle или другие здани€ без отдельной панели.
/// </summary>
public sealed class BuildingActionPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("UI")]
    [SerializeField] private TMP_Text _titleText;

    [Header("Demolish")]
    [SerializeField] private GameObject _demolishButtonRoot;
    [SerializeField] private Button _demolishButton;
    [SerializeField] private TMP_Text _cannotDemolishText;

    private BuildingBase _currentBuilding;
    private SelectionSystem _selectionSystem;

    [Inject]
    private void Construct(SelectionSystem selectionSystem)
    {
        _selectionSystem = selectionSystem;
    }

    private void OnEnable()
    {
        _demolishButton.onClick.AddListener(DemolishBuilding);
    }

    private void OnDisable()
    {
        _demolishButton.onClick.RemoveListener(DemolishBuilding);
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));
        valid &= ValidationUtility.IsAssigned(this, _titleText, nameof(_titleText));
        valid &= ValidationUtility.IsAssigned(this, _demolishButtonRoot, nameof(_demolishButtonRoot));
        valid &= ValidationUtility.IsAssigned(this, _demolishButton, nameof(_demolishButton));
        valid &= ValidationUtility.IsAssigned(this, _cannotDemolishText, nameof(_cannotDemolishText));

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
        _panelTween.Show();
    }

    public void Hide()
    {
        _currentBuilding = null;

        ClearView();
        _panelTween.Hide();
    }

    private void Refresh()
    {
        if (_currentBuilding == null)
        {
            ClearView();
            return;
        }

        _titleText.text = _currentBuilding.DisplayName;

        RefreshDemolishView();
    }

    private void RefreshDemolishView()
    {
        bool canDemolish = _currentBuilding != null &&
                           _currentBuilding.CanBeDemolishedByButton;

        _demolishButtonRoot.SetActive(canDemolish);
        _demolishButton.interactable = canDemolish;

        _cannotDemolishText.gameObject.SetActive(!canDemolish);
        _cannotDemolishText.text = "Ёто здание снести нельз€";
    }

    private void DemolishBuilding()
    {
        if (_currentBuilding == null)
            return;

        BuildingBase building = _currentBuilding;

        if (!building.TryDemolishByButton())
        {
            RefreshDemolishView();
            return;
        }

        if (_selectionSystem != null)
            _selectionSystem.ClearSelection();
        else
            Hide();
    }

    private void ClearView()
    {
        _titleText.text = string.Empty;

        _demolishButton.interactable = false;
        _demolishButtonRoot.SetActive(false);

        _cannotDemolishText.text = string.Empty;
        _cannotDemolishText.gameObject.SetActive(false);
    }
}