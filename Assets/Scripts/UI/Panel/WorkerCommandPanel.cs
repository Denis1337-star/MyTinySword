using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI-панель выбранного worker
/// </summary>
public class WorkerCommandPanel : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Text")]
    [SerializeField] private TMP_Text _titleText;

    [Header("Buttons")]
    [SerializeField] private Button _chopWoodButton;
    [SerializeField] private Button _mineGoldButton;
    [SerializeField] private Button _huntMeatButton;
    [SerializeField] private Button _idleButton;

    private WorkerCommandService _workerCommandService;
    private Worker _currentWorker;

    [Inject]
    private void Construct(WorkerCommandService workerCommandService)
    {
        _workerCommandService = workerCommandService;
    }

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        BindButtons();
    }

    private void OnDisable()
    {
        UnbindButtons();
    }

    public void ShowForWorker(Worker worker)
    {
        if (worker == null)
        {
            Hide();
            return;
        }

        _currentWorker = worker;

        if (_root != null)
            _root.SetActive(true);

        RefreshTitle();
    }

    public void Hide()
    {
        _currentWorker = null;

        if (_root != null)
            _root.SetActive(false);
    }

    private void BindButtons()
    {
        _chopWoodButton?.onClick.AddListener(SetChopWood);
        _mineGoldButton?.onClick.AddListener(SetMineGold);
        _huntMeatButton?.onClick.AddListener(SetHuntMeat);
        _idleButton?.onClick.AddListener(SetIdle);
    }

    private void UnbindButtons()
    {
        _chopWoodButton?.onClick.RemoveListener(SetChopWood);
        _mineGoldButton?.onClick.RemoveListener(SetMineGold);
        _huntMeatButton?.onClick.RemoveListener(SetHuntMeat);
        _idleButton?.onClick.RemoveListener(SetIdle);
    }

    private void SetChopWood()
    {
        AssignJob(WorkerJobType.ChopWood);
    }

    private void SetMineGold()
    {
        AssignJob(WorkerJobType.MineGold);
    }

    private void SetHuntMeat()
    {
        AssignJob(WorkerJobType.HuntMeat);
    }

    private void SetIdle()
    {
        AssignJob(WorkerJobType.None);
    }

    private void AssignJob(WorkerJobType job)
    {
        if (_currentWorker == null || _workerCommandService == null)
            return;

        _workerCommandService.TryAssignJob(_currentWorker, job);
        RefreshTitle();
    }

    private void RefreshTitle()
    {
        if (_titleText == null)
            return;

        if (_currentWorker == null)
        {
            _titleText.text = string.Empty;
            return;
        }

        _titleText.text = _currentWorker.CurrentJob.ToString();
    }
}
