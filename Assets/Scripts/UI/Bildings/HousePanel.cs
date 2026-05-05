using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI-панель дома
/// Показывает количество рабочих, стоимость найма
/// </summary>
public class HousePanel : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Text")]
    [SerializeField] private TMP_Text _workersLimitText;
    [SerializeField] private TMP_Text _hireCostText;

    [Header("Buttons")]
    [SerializeField] private Button _hireButton;

    private House _currentHouse;
    private WorkerListPanel _workerListPanel;

    [Inject]
    private void Construct(WorkerListPanel workerListPanel)
    {
        _workerListPanel = workerListPanel;
    }

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        _hireButton?.onClick.AddListener(HireWorker);
    }

    private void OnDisable()
    {
        _hireButton?.onClick.RemoveListener(HireWorker);
        UnsubscribeFromHouse();
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

        if (_root != null)
            _root.SetActive(false);
    }

    private void HireWorker()
    {
        if (_currentHouse == null)
            return;

        _currentHouse.HireWorker();
        Refresh();
    }

    private void Refresh()
    {
        if (_currentHouse == null)
        {
            ClearText();
            return;
        }

        if (_workersLimitText != null)
        {
            _workersLimitText.text =
                $"Нанято: {_currentHouse.CurrentWorkers}/{_currentHouse.MaxWorkers}";
        }

        if (_hireCostText != null)
        {
            _hireCostText.text =
                $"Стоимость: Wood {_currentHouse.CurrentWoodCost} / Gold {_currentHouse.CurrentGoldCost}";
        }

        if (_hireButton != null)
            _hireButton.interactable = _currentHouse.CanHire();
    }

    private void ClearText()
    {
        if (_workersLimitText != null)
            _workersLimitText.text = "Нанято: 0/0";

        if (_hireCostText != null)
            _hireCostText.text = "Стоимость: -";

        if (_hireButton != null)
            _hireButton.interactable = false;
    }

    private void ShowRoot()
    {
        if (_root != null)
            _root.SetActive(true);
    }

    private void UnsubscribeFromHouse()
    {
        if (_currentHouse == null)
            return;

        _currentHouse.OnWorkersChanged -= Refresh;
    }
}
