using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

/// <summary>
/// UI панель выбранного worker
/// </summary>
public sealed class WorkerCommandPanel : ValidatedMonoBehaviour
{
    [SerializeField] private TMP_Text _currentJobText;
    [SerializeField] private TMP_Text _pendingJobText;
    [SerializeField] private Button _chopWoodButton;
    [SerializeField] private Button _mineGoldButton;
    [SerializeField] private Button _huntMeatButton;
    [SerializeField] private SimplePanelTween _panelTween;

    private readonly EntityEventSubscription<Worker> _workerEvents = new();

    private Worker _currentWorker;

    public SimplePanelTween PanelTween => _panelTween;

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _currentJobText, nameof(_currentJobText));
        valid &= ValidationUtility.IsAssigned(this, _pendingJobText, nameof(_pendingJobText));
        valid &= ValidationUtility.IsAssigned(this, _chopWoodButton, nameof(_chopWoodButton));
        valid &= ValidationUtility.IsAssigned(this, _mineGoldButton, nameof(_mineGoldButton));
        valid &= ValidationUtility.IsAssigned(this, _huntMeatButton, nameof(_huntMeatButton));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));

        return valid;
    }

    private void OnEnable()
    {
        SubscribeButtons();
        BindCurrentWorkerEvents();
        YG2.onSwitchLang += HandleSwitchLang;
        Refresh();
    }

    private void OnDisable()
    {
        UnsubscribeButtons();
        ClearWorkerSubscription();
        YG2.onSwitchLang -= HandleSwitchLang;
    }

    private void HandleSwitchLang(string lang)
    {
        Refresh();
    }

    public void ShowForWorker(Worker worker)
    {
        if (worker == null)
        {
            Hide();
            return;
        }

        if (_workerEvents.IsBoundTo(worker))
        {
            Refresh();
            return;
        }

        ClearWorkerSubscription();

        _currentWorker = worker;

        if (gameObject.activeInHierarchy)
            BindCurrentWorkerEvents();

        Refresh();
    }

    public void Hide()
    {
        ClearWorkerSubscription();

        _currentWorker = null;

        ClearText();
        SetButtonsInteractable(false);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        _chopWoodButton.interactable = interactable;
        _mineGoldButton.interactable = interactable;
        _huntMeatButton.interactable = interactable;
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
        _currentWorker.AssignJob(job);
        Refresh();
    }

    private void Refresh()
    {
        if (_currentWorker == null)
        {
            ClearText();
            SetButtonsInteractable(false);
            return;
        }

        _currentJobText.text =
            GameUiText.CurrentJob(WorkerJobLocalization.GetName(_currentWorker.CurrentJob));

        _pendingJobText.text = _currentWorker.HasPendingJob
            ? GameUiText.NextJob(WorkerJobLocalization.GetName(_currentWorker.PendingJob))
            : GameUiText.NextJobNone;

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
        if (_currentWorker.CurrentJob == job && !_currentWorker.HasPendingJob)
            return false;

        if (_currentWorker.PendingJob == job)
            return false;

        return true;
    }

    private void ClearText()
    {
        _currentJobText.text = GameUiText.CurrentJobNone;
        _pendingJobText.text = GameUiText.NextJobNone;
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

    private void BindCurrentWorkerEvents()
    {
        _workerEvents.Bind(
            _currentWorker,
            w =>
            {
                w.OnJobChanged += Refresh;
                w.OnActivityChanged += Refresh;
            },
            w =>
            {
                w.OnJobChanged -= Refresh;
                w.OnActivityChanged -= Refresh;
            });
    }

    private void ClearWorkerSubscription()
    {
        _workerEvents.Clear(w =>
        {
            w.OnJobChanged -= Refresh;
            w.OnActivityChanged -= Refresh;
        });
    }
}
