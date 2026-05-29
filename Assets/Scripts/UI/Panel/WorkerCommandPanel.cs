using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI панель выбранного worker
/// </summary>
public sealed class WorkerCommandPanel : ValidatedMonoBehaviour
{
    [Header("Job Text")]
    [SerializeField] private TMP_Text _currentJobText;
    [SerializeField] private TMP_Text _pendingJobText;

    [Header("Buttons")]
    [SerializeField] private Button _chopWoodButton;
    [SerializeField] private Button _mineGoldButton;
    [SerializeField] private Button _huntMeatButton;

    private Worker _currentWorker;
    private Worker _subscribedWorker;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _currentJobText, nameof(_currentJobText));
        valid &= ValidationUtility.IsAssigned(this, _pendingJobText, nameof(_pendingJobText));
        valid &= ValidationUtility.IsAssigned(this, _chopWoodButton, nameof(_chopWoodButton));
        valid &= ValidationUtility.IsAssigned(this, _mineGoldButton, nameof(_mineGoldButton));
        valid &= ValidationUtility.IsAssigned(this, _huntMeatButton, nameof(_huntMeatButton));

        return valid;
    }

    private void OnEnable()
    {
        SubscribeButtons();
        SubscribeToCurrentWorker();
        Refresh();
    }

    private void OnDisable()
    {
        UnsubscribeButtons();
        UnsubscribeFromCurrentWorker();
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
            UnsubscribeFromCurrentWorker();
            _currentWorker = worker;

            if (gameObject.activeInHierarchy)
                SubscribeToCurrentWorker();
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        Refresh();
    }

    public void Hide()
    {
        UnsubscribeFromCurrentWorker();

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
        if (_currentWorker == null)
            return;

        _currentWorker.AssignJob(job);
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

        _currentJobText.text =
            $"Текущая работа: {WorkerJobLocalization.GetName(_currentWorker.CurrentJob)}";

        _pendingJobText.text = _currentWorker.HasPendingJob
            ? $"Следующая работа: {WorkerJobLocalization.GetName(_currentWorker.PendingJob)}"
            : "Следующая работа: нет";

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        _chopWoodButton.interactable = CanSelectJob(WorkerJobType.ChopWood);
        _mineGoldButton.interactable = CanSelectJob(WorkerJobType.MineGold);
        _huntMeatButton.interactable = CanSelectJob(WorkerJobType.HuntMeat);
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

    private void ClearText()
    {
        _currentJobText.text = "Текущая работа: нет";
        _pendingJobText.text = "Следующая работа: нет";
    }

    private void SubscribeButtons()
    {
        _chopWoodButton.onClick.AddListener(OnChopWoodClicked);
        _mineGoldButton.onClick.AddListener(OnMineGoldClicked);
        _huntMeatButton.onClick.AddListener(OnHuntMeatClicked);
    }

    private void UnsubscribeButtons()
    {
        _chopWoodButton.onClick.RemoveListener(OnChopWoodClicked);
        _mineGoldButton.onClick.RemoveListener(OnMineGoldClicked);
        _huntMeatButton.onClick.RemoveListener(OnHuntMeatClicked);
    }

    private void SubscribeToCurrentWorker()
    {
        if (_currentWorker == null)
            return;

        if (_subscribedWorker == _currentWorker)
            return;

        _currentWorker.OnJobChanged += Refresh;
        _currentWorker.OnActivityChanged += Refresh;

        _subscribedWorker = _currentWorker;
    }

    private void UnsubscribeFromCurrentWorker()
    {
        if (_subscribedWorker == null)
            return;

        _subscribedWorker.OnJobChanged -= Refresh;
        _subscribedWorker.OnActivityChanged -= Refresh;

        _subscribedWorker = null;
    }
}