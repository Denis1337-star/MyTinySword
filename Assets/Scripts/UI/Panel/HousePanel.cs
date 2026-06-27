using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI-панель дома: рабочие, стоимость найма, кнопки назначения.
/// </summary>
public sealed class HousePanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("Info")]
    [SerializeField] private TMP_Text _workersLimitText;
    [SerializeField] private TMP_Text _hireCostText;

    [Header("Actions")]
    [SerializeField] private Button _hireButton;
    [SerializeField] private Button _demolishButton;

    [Header("Assign All Workers")]
    [SerializeField] private Button _assignAllWoodButton;
    [SerializeField] private Button _assignAllGoldButton;
    [SerializeField] private Button _assignAllMeatButton;

    private readonly EntityEventSubscription<House> _houseEvents = new();

    private House _currentHouse;
    private WorkerListPanel _workerListPanel;
    private BuildingDemolishService _buildingDemolishService;

    public event Action<WorkerJobType> AllWorkersJobAssigned;

    public RectTransform AssignAllWoodButtonRect => _assignAllWoodButton.transform as RectTransform;

    public RectTransform AssignAllGoldButtonRect => _assignAllGoldButton.transform as RectTransform;

    public RectTransform AssignAllMeatButtonRect => _assignAllMeatButton.transform as RectTransform;

    public SimplePanelTween PanelTween => _panelTween;

    [Inject]
    private void Construct(
        WorkerListPanel workerListPanel,
        BuildingDemolishService buildingDemolishService)
    {
        _workerListPanel = workerListPanel;
        _buildingDemolishService = buildingDemolishService;
    }

    private void OnEnable()
    {
        _hireButton.onClick.AddListener(HireWorker);
        _demolishButton.onClick.AddListener(RequestDemolishHouse);

        _assignAllWoodButton.onClick.AddListener(AssignAllToWood);
        _assignAllGoldButton.onClick.AddListener(AssignAllToGold);
        _assignAllMeatButton.onClick.AddListener(AssignAllToMeat);
    }

    private void OnDisable()
    {
        _hireButton.onClick.RemoveListener(HireWorker);
        _demolishButton.onClick.RemoveListener(RequestDemolishHouse);

        _assignAllWoodButton.onClick.RemoveListener(AssignAllToWood);
        _assignAllGoldButton.onClick.RemoveListener(AssignAllToGold);
        _assignAllMeatButton.onClick.RemoveListener(AssignAllToMeat);

        ClearHouseSubscription();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));
        valid &= ValidationUtility.IsAssigned(this, _workersLimitText, nameof(_workersLimitText));
        valid &= ValidationUtility.IsAssigned(this, _hireCostText, nameof(_hireCostText));
        valid &= ValidationUtility.IsAssigned(this, _hireButton, nameof(_hireButton));
        valid &= ValidationUtility.IsAssigned(this, _demolishButton, nameof(_demolishButton));
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

        if (_houseEvents.IsBoundTo(house))
        {
            Refresh();
            _workerListPanel.Show(house);
            return;
        }

        ClearHouseSubscription();

        _currentHouse = house;
        _houseEvents.Bind(
            house,
            h => h.OnWorkersChanged += HandleHouseWorkersChanged,
            h => h.OnWorkersChanged -= HandleHouseWorkersChanged);

        _workerListPanel.Show(house);
        Refresh();
    }

    public void Hide()
    {
        ClearHouseSubscription();

        _currentHouse = null;

        _workerListPanel.Hide();

        ClearText();
    }

    private void HireWorker()
    {
        _currentHouse.HireWorker();
        Refresh();
    }

    private void RequestDemolishHouse()
    {
        _buildingDemolishService.RequestDemolish(_currentHouse);
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
        _currentHouse.AssignAllWorkersToJob(job);

        AllWorkersJobAssigned?.Invoke(job);

        Refresh();
    }

    private void HandleHouseWorkersChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_currentHouse == null)
        {
            ClearText();
            return;
        }

        _workersLimitText.text =
            $"Рабочие: {_currentHouse.CurrentWorkers}/{_currentHouse.MaxWorkers}";

        _hireCostText.text =
            $"Стоимость: дерево {_currentHouse.CurrentWoodCost} / золото {_currentHouse.CurrentGoldCost}";

        _hireButton.interactable = _currentHouse.CanHire();

        BuildingDemolishRules.RefreshButton(_demolishButton, _currentHouse);
        RefreshAssignAllButtons();

        _workerListPanel.Rebuild();
    }

    private void RefreshAssignAllButtons()
    {
        bool hasWorkers = _currentHouse.CurrentWorkers > 0;

        _assignAllWoodButton.interactable = hasWorkers;
        _assignAllGoldButton.interactable = hasWorkers;
        _assignAllMeatButton.interactable = hasWorkers;
    }

    private void ClearText()
    {
        _workersLimitText.text = "Рабочие: 0/0";
        _hireCostText.text = "Стоимость: -";

        _hireButton.interactable = false;
        _demolishButton.gameObject.SetActive(false);

        _assignAllWoodButton.interactable = false;
        _assignAllGoldButton.interactable = false;
        _assignAllMeatButton.interactable = false;
    }

    private void ClearHouseSubscription()
    {
        _houseEvents.Clear(h => h.OnWorkersChanged -= HandleHouseWorkersChanged);
    }
}
