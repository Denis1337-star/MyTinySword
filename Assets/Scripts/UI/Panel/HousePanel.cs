using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI панель дома.
/// Показывает рабочих, стоимость найма, кнопку найма и кнопку ручного сноса,
/// если конкретный дом разрешено снести через кнопку.
/// </summary>
public sealed class HousePanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Info")]
    [SerializeField] private TMP_Text _workersLimitText;
    [SerializeField] private TMP_Text _hireCostText;

    [Header("Actions")]
    [SerializeField] private Button _hireButton;
    [SerializeField] private Button _demolishButton;
    [SerializeField] private GameObject _demolishButtonRoot;

    [Header("Assign All Workers")]
    [SerializeField] private Button _assignAllWoodButton;
    [SerializeField] private Button _assignAllGoldButton;
    [SerializeField] private Button _assignAllMeatButton;

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

        _assignAllWoodButton.onClick.AddListener(AssignAllToWood);
        _assignAllGoldButton.onClick.AddListener(AssignAllToGold);
        _assignAllMeatButton.onClick.AddListener(AssignAllToMeat);

    }

    private void OnDisable()
    {
        _hireButton.onClick.RemoveListener(HireWorker);
        _demolishButton.onClick.RemoveListener(DemolishHouse);

        _assignAllWoodButton.onClick.RemoveListener(AssignAllToWood);
        _assignAllGoldButton.onClick.RemoveListener(AssignAllToGold);
        _assignAllMeatButton.onClick.RemoveListener(AssignAllToMeat);

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
        valid &= ValidationUtility.IsAssigned(this, _demolishButtonRoot, nameof(_demolishButtonRoot));
        valid &= ValidationUtility.IsAssigned(this, _assignAllWoodButton, nameof(_assignAllWoodButton));
        valid &= ValidationUtility.IsAssigned(this, _assignAllGoldButton, nameof(_assignAllGoldButton));
        valid &= ValidationUtility.IsAssigned(this, _assignAllMeatButton, nameof(_assignAllMeatButton));

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

        if (!house.TryDemolishByButton())
        {
            RefreshDemolishButton();
            return;
        }

        if (_selectionSystem != null)
            _selectionSystem.ClearSelection();
        else
            Hide();
    }
    private void AssignAllToWood()
    {
        AssignAllWorkersToJob(WorkerJobType.ChopWood);
    }

    private void AssignAllToGold()
    {
        AssignAllWorkersToJob(WorkerJobType.MineGold);
    }

    private void AssignAllToMeat()
    {
        AssignAllWorkersToJob(WorkerJobType.HuntMeat);
    }
    private void AssignAllWorkersToJob(WorkerJobType job)
    {
        if (_currentHouse == null)
            return;

        _currentHouse.AssignAllWorkersToJob(job);

        Refresh();
        _workerListPanel?.Show(_currentHouse);
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

        RefreshDemolishButton();
        RefreshAssignAllButtons();
    }

    private void RefreshDemolishButton()
    {
        bool canDemolish = _currentHouse != null &&
                           _currentHouse.CanBeDemolishedByButton;

        _demolishButtonRoot.SetActive(canDemolish);
        _demolishButton.interactable = canDemolish;
    }
    private void RefreshAssignAllButtons()
    {
        bool hasWorkers = _currentHouse != null &&
                          _currentHouse.CurrentWorkers > 0;

        _assignAllWoodButton.interactable = hasWorkers;
        _assignAllGoldButton.interactable = hasWorkers;
        _assignAllMeatButton.interactable = hasWorkers;
    }

    private void ClearText()
    {
        _workersLimitText.text = "Нанято: 0/0";
        _hireCostText.text = "Стоимость: -";

        _hireButton.interactable = false;

        _demolishButton.interactable = false;
        _demolishButtonRoot.SetActive(false);

        _assignAllWoodButton.interactable = false;
        _assignAllGoldButton.interactable = false;
        _assignAllMeatButton.interactable = false;
    }

    private void ShowRoot()
    {
        _root.SetActive(true);
        RefreshDemolishButton();

        RefreshAssignAllButtons();
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