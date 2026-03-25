using UnityEngine;
using UnityEngine.UI;

public class WorkerCommandPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button chopWoodButton;
    [SerializeField] private Button mineGoldButton;
    [SerializeField] private Button huntMeatButton;

    [Header("Services")]
    [SerializeField] private WorkerCommandService workerCommandService;

    [Header("Info")]
    [SerializeField] private Text currentJobText;
    [SerializeField] private Text pendingJobText;
    [SerializeField] private Text stateText;
    [SerializeField] private Text workerNameText;

    [Header("Behaviour")]
    [SerializeField] private bool closeAfterAssign = false;

    private Worker currentWorker;

    private void Awake()
    {
        if (chopWoodButton != null)
            chopWoodButton.onClick.AddListener(OnChopWoodClicked);

        if (mineGoldButton != null)
            mineGoldButton.onClick.AddListener(OnMineGoldClicked);

        if (huntMeatButton != null)
            huntMeatButton.onClick.AddListener(OnHuntMeatClicked);

        gameObject.SetActive(false);
    }

    public void ShowForWorker(Worker worker)
    {
        if (worker == null)
            return;

        if (currentWorker == worker && gameObject.activeSelf)
        {
            Refresh();
            return;
        }

        UnsubscribeFromWorker();

        currentWorker = worker;
        SubscribeToWorker();

        gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        UnsubscribeFromWorker();
        currentWorker = null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        UnsubscribeFromWorker();
    }

    private void SubscribeToWorker()
    {
        if (currentWorker == null)
            return;

        currentWorker.OnJobChanged += Refresh;
        currentWorker.OnActivityChanged += Refresh;
    }

    private void UnsubscribeFromWorker()
    {
        if (currentWorker == null)
            return;

        currentWorker.OnJobChanged -= Refresh;
        currentWorker.OnActivityChanged -= Refresh;
    }

    private void Refresh()
    {
        if (currentWorker == null)
            return;

        if (workerNameText != null)
            workerNameText.text = currentWorker.name;

        if (currentJobText != null)
            currentJobText.text = $"Текущая работа: {WorkerJobLocalization.GetName(currentWorker.CurrentJob)}";

        if (pendingJobText != null)
        {
            string pending = currentWorker.HasPendingJob
                ? WorkerJobLocalization.GetName(currentWorker.PendingJob)
                : "Нет";

            pendingJobText.text = $"Следующая работа: {pending}";
        }

        if (stateText != null)
            stateText.text = $"Состояние: {GetReadableState(currentWorker.CurrentStateName)}";

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        if (currentWorker == null)
            return;

        if (chopWoodButton != null)
        {
            chopWoodButton.interactable =
                currentWorker.CurrentJob != WorkerJobType.ChopWood &&
                currentWorker.PendingJob != WorkerJobType.ChopWood;
        }

        if (mineGoldButton != null)
        {
            mineGoldButton.interactable =
                currentWorker.CurrentJob != WorkerJobType.MineGold &&
                currentWorker.PendingJob != WorkerJobType.MineGold;
        }

        if (huntMeatButton != null)
        {
            huntMeatButton.interactable =
                currentWorker.CurrentJob != WorkerJobType.HuntMeat &&
                currentWorker.PendingJob != WorkerJobType.HuntMeat;
        }
    }

    private string GetReadableState(string stateName)
    {
        return stateName switch
        {
            nameof(WorkerIdleState) => "Ожидает",
            nameof(WorkerFindResourceState) => "Ищет ресурс",
            nameof(WorkerGoToResourceState) => "Идёт к ресурсу",
            nameof(WorkerWorkState) => "Работает",
            nameof(WorkerCarryState) => "Несёт ресурс",
            _ => stateName
        };
    }

    public void OnChopWoodClicked()
    {
        AssignJob(WorkerJobType.ChopWood);
    }

    public void OnMineGoldClicked()
    {
        AssignJob(WorkerJobType.MineGold);
    }

    public void OnHuntMeatClicked()
    {
        AssignJob(WorkerJobType.HuntMeat);
    }

    private void AssignJob(WorkerJobType job)
    {
        if (currentWorker == null || workerCommandService == null)
            return;

        bool assigned = workerCommandService.TryAssignJob(currentWorker, job);
        if (!assigned)
            return;

        Refresh();

        if (closeAfterAssign)
            Hide();
    }
}
