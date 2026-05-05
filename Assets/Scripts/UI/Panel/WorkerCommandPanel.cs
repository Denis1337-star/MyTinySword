using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI-панель выбранного worker.
/// Показывает текущую/следующую работу и кнопки назначения jobs.
/// </summary>
public class WorkerCommandPanel : MonoBehaviour
{
    [Header("Job Text")]
    [SerializeField] private TMP_Text _currentJobText;
    [SerializeField] private TMP_Text _pendingJobText;

    [Header("Buttons")]
    [SerializeField] private Button _chopWoodButton;
    [SerializeField] private Button _mineGoldButton;
    [SerializeField] private Button _huntMeatButton;

    private WorkerCommandService _workerCommandService;
    private Worker _currentWorker;

    private bool _buttonsSubscribed;
    private bool _workerSubscribed;

    [Inject]
    private void Construct(WorkerCommandService workerCommandService)
    {
        _workerCommandService = workerCommandService;
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SubscribeButtons();
        SubscribeToWorker();
        Refresh();
    }

    private void OnDisable()
    {
        UnsubscribeButtons();
        UnsubscribeFromWorker();
    }

    public void ShowForWorker(Worker worker)
    {
        if (worker == null)
        {
            Hide();
            return;
        }

        if (_currentWorker != worker)
        {
            UnsubscribeFromWorker();
            _currentWorker = worker;

            if (gameObject.activeInHierarchy)
                SubscribeToWorker();
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        Refresh();
    }

    public void Hide()
    {
        UnsubscribeFromWorker();

        _currentWorker = null;

        ClearText();
        RefreshButtons();

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private void OnChopWoodClicked()
    {
        AssignJob(WorkerJobType.ChopWood);
    }

    private void OnMineGoldClicked()
    {
        AssignJob(WorkerJobType.MineGold);
    }

    private void OnHuntMeatClicked()
    {
        AssignJob(WorkerJobType.HuntMeat);
    }

    private void AssignJob(WorkerJobType job)
    {
        if (_currentWorker == null || _workerCommandService == null)
            return;

        bool assigned = _workerCommandService.TryAssignJob(_currentWorker, job);

        if (assigned)
            Refresh();
    }

    private void Refresh()
    {
        if (_currentWorker == null)
        {
            ClearText();
            RefreshButtons();
            return;
        }

        if (_currentJobText != null)
        {
            _currentJobText.text =
                $"Текущая работа: {WorkerJobLocalization.GetName(_currentWorker.CurrentJob)}";
        }

        if (_pendingJobText != null)
        {
            _pendingJobText.text = _currentWorker.HasPendingJob
                ? $"Следующая работа: {WorkerJobLocalization.GetName(_currentWorker.PendingJob)}"
                : "Следующая работа: нет";
        }

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        SetButtonInteractable(_chopWoodButton, CanSelectJob(WorkerJobType.ChopWood));
        SetButtonInteractable(_mineGoldButton, CanSelectJob(WorkerJobType.MineGold));
        SetButtonInteractable(_huntMeatButton, CanSelectJob(WorkerJobType.HuntMeat));
    }

    private bool CanSelectJob(WorkerJobType job)
    {
        if (_currentWorker == null)
            return false;

        if (_currentWorker.CurrentJob == job && !_currentWorker.HasPendingJob)
            return false;

        if (_currentWorker.PendingJob == job)
            return false;

        return true;
    }

    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button == null)
            return;

        button.interactable = interactable;
    }

    private void ClearText()
    {
        if (_currentJobText != null)
            _currentJobText.text = "Текущая работа: нет";

        if (_pendingJobText != null)
            _pendingJobText.text = "Следующая работа: нет";
    }

    private void SubscribeButtons()
    {
        if (_buttonsSubscribed)
            return;

        _chopWoodButton?.onClick.AddListener(OnChopWoodClicked);
        _mineGoldButton?.onClick.AddListener(OnMineGoldClicked);
        _huntMeatButton?.onClick.AddListener(OnHuntMeatClicked);

        _buttonsSubscribed = true;
    }

    private void UnsubscribeButtons()
    {
        if (!_buttonsSubscribed)
            return;

        _chopWoodButton?.onClick.RemoveListener(OnChopWoodClicked);
        _mineGoldButton?.onClick.RemoveListener(OnMineGoldClicked);
        _huntMeatButton?.onClick.RemoveListener(OnHuntMeatClicked);

        _buttonsSubscribed = false;
    }

    private void SubscribeToWorker()
    {
        if (_workerSubscribed)
            return;

        if (_currentWorker == null)
            return;

        _currentWorker.OnJobChanged += Refresh;
        _currentWorker.OnActivityChanged += Refresh;

        _workerSubscribed = true;
    }

    private void UnsubscribeFromWorker()
    {
        if (!_workerSubscribed)
            return;

        if (_currentWorker != null)
        {
            _currentWorker.OnJobChanged -= Refresh;
            _currentWorker.OnActivityChanged -= Refresh;
        }

        _workerSubscribed = false;
    }
}