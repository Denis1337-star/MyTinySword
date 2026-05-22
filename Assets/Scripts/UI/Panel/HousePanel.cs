using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI панель дома
/// </summary>
public sealed class HousePanel : ValidatedMonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _workersLimitText;
    [SerializeField] private TMP_Text _hireCostText;
    [SerializeField] private Button _hireButton;
    [SerializeField] private Button _demolishButton;

    private House _currentHouse;
    private WorkerListPanel _workerListPanel;
    private SelectionSystem _selectionSystem;

    [Inject]
    private void Construct(
        WorkerListPanel workerListPanel,
        SelectionSystem selectionSystem)
    {
        _workerListPanel = workerListPanel;
        _selectionSystem = selectionSystem;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        _hireButton.onClick.AddListener(HireWorker);
        _demolishButton.onClick.AddListener(DemolishHouse);
    }

    private void OnDisable()
    {
        _hireButton.onClick.RemoveListener(HireWorker);
        _demolishButton.onClick.RemoveListener(DemolishHouse);

        UnsubscribeFromHouse();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _workersLimitText, nameof(_workersLimitText));
        valid &= ValidationUtility.IsAssigned(this, _hireCostText, nameof(_hireCostText));
        valid &= ValidationUtility.IsAssigned(this, _hireButton, nameof(_hireButton));
        valid &= ValidationUtility.IsAssigned(this, _demolishButton, nameof(_demolishButton));

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
            Refresh();
            _workerListPanel?.Show(house);
            return;
        }

        UnsubscribeFromHouse();

        _currentHouse = house;
        _currentHouse.OnWorkersChanged += Refresh;

        ShowRoot();
        Refresh();

        _workerListPanel?.Show(house);
    }

    public void Hide()
    {
        UnsubscribeFromHouse();

        _currentHouse = null;

        _workerListPanel?.Hide();

        ClearText();
        HideRoot();
    }

    private void HireWorker()
    {
        if (_currentHouse == null)
            return;

        _currentHouse.HireWorker();
        Refresh();
    }

    private void DemolishHouse()
    {
        if (_currentHouse == null)
            return;

        House house = _currentHouse;

        if (_selectionSystem != null)
            _selectionSystem.ClearSelection();
        else
            Hide();

        house.Demolish();
    }

    private void Refresh()
    {
        if (_currentHouse == null)
        {
            ClearText();
            return;
        }

        _workersLimitText.text =
            $"Нанято: {_currentHouse.CurrentWorkers}/{_currentHouse.MaxWorkers}";

        _hireCostText.text =
            $"Стоимость: Дерево {_currentHouse.CurrentWoodCost} / Золото {_currentHouse.CurrentGoldCost}";

        _hireButton.interactable = _currentHouse.CanHire();
        _demolishButton.interactable = true;
    }

    private void ClearText()
    {
        _workersLimitText.text = "Нанято: 0/0";
        _hireCostText.text = "Стоимость: -";

        _hireButton.interactable = false;
        _demolishButton.interactable = false;
    }

    private void ShowRoot()
    {
        _root.SetActive(true);
    }

    private void HideRoot()
    {
        _root.SetActive(false);
    }

    private void UnsubscribeFromHouse()
    {
        if (_currentHouse == null)
            return;

        _currentHouse.OnWorkersChanged -= Refresh;
    }
}