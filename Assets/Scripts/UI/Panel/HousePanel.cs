using UnityEngine;
using UnityEngine.UI;

public class HousePanel : MonoBehaviour
{
    [SerializeField] private Text limitText;
    [SerializeField] private Button hireButton;
    [SerializeField] private WorkerListPanel workerList;
    [SerializeField] private Text costText;

    private House currentHouse;
    private ResourceStorage subscribedStorage;

    private void Awake()
    {
        if (hireButton != null)
            hireButton.onClick.AddListener(OnHireClicked);

        gameObject.SetActive(false);
    }

    public void Show(House house)
    {
        if (house == null)
            return;

        if (currentHouse == house && gameObject.activeSelf)
        {
            Refresh();
            return;
        }

        Hide();

        currentHouse = house;
        Subscribe();
        gameObject.SetActive(true);

        if (workerList != null)
            workerList.Bind(currentHouse);

        Refresh();
    }

    public void Hide()
    {
        Unsubscribe();

        currentHouse = null;
        gameObject.SetActive(false);

        if (workerList != null)
            workerList.Bind(null);
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (currentHouse != null)
            currentHouse.OnWorkersChanged += Refresh;

        ResourceStorage storage = ResourceStorage.Instance;
        if (storage != null)
        {
            subscribedStorage = storage;
            subscribedStorage.OnResourcesChanged += Refresh;
        }
    }

    private void Unsubscribe()
    {
        if (currentHouse != null)
            currentHouse.OnWorkersChanged -= Refresh;

        if (subscribedStorage != null)
        {
            subscribedStorage.OnResourcesChanged -= Refresh;
            subscribedStorage = null;
        }
    }

    private void Refresh()
    {
        if (currentHouse == null)
            return;

        if (limitText != null)
            limitText.text = $"Рабочие: {currentHouse.CurrentWorkers} / {currentHouse.MaxWorkers}";

        if (costText != null)
        {
            int currentWood = ResourceStorage.Instance != null ? ResourceStorage.Instance.Wood : 0;
            int currentGold = ResourceStorage.Instance != null ? ResourceStorage.Instance.Gold : 0;

            costText.text =
                $"Стоимость найма\n" +
                $"Дерево: {currentWood} / {currentHouse.CurrentWoodCost}\n" +
                $"Золото: {currentGold} / {currentHouse.CurrentGoldCost}";
        }

        if (hireButton != null)
            hireButton.interactable = currentHouse.CanHire();

        if (workerList != null)
            workerList.Refresh();
    }

    public void OnHireClicked()
    {
        if (currentHouse == null)
            return;

        currentHouse.HireWorker();
        Refresh();
    }
}
