using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-панель выбранного дома
/// </summary>
public class HousePanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _workersText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _blockReasonText;
    [SerializeField] private Button _hireButton;

    private House _currentHouse;

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        if (_hireButton != null)
            _hireButton.onClick.AddListener(HireWorker);
    }

    private void OnDisable()
    {
        if (_hireButton != null)
            _hireButton.onClick.RemoveListener(HireWorker);

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
            Refresh();
            ShowRoot();
            return;
        }

        UnsubscribeFromHouse();

        _currentHouse = house;
        _currentHouse.OnWorkersChanged += Refresh;

        ShowRoot();
        Refresh();
    }

    public void Hide()
    {
        UnsubscribeFromHouse();

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
            return;

        if (_workersText != null)
        {
            _workersText.text =
                $"{_currentHouse.CurrentWorkers}/{_currentHouse.MaxWorkers}";
        }

        if (_costText != null)
        {
            _costText.text =
                $"Wood: {_currentHouse.CurrentWoodCost}  Gold: {_currentHouse.CurrentGoldCost}";
        }

        string blockReason = _currentHouse.GetHireBlockReason();

        if (_blockReasonText != null)
            _blockReasonText.text = blockReason;

        if (_hireButton != null)
            _hireButton.interactable = string.IsNullOrEmpty(blockReason);
    }

    private void ShowRoot()
    {
        if (_root != null)
            _root.SetActive(true);
    }

    private void UnsubscribeFromHouse()
    {
        if (_currentHouse != null)
            _currentHouse.OnWorkersChanged -= Refresh;

        _currentHouse = null;
    }
}
