using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI-панель выбранного дома.
/// Показывает лимит worker'ов, стоимость найма,
/// позволяет нанять нового worker'а и отображает список worker'ов дома.
/// </summary>
public class HousePanel : MonoBehaviour
{
    [SerializeField] private Text limitText;
    [SerializeField] private Button hireButton;
    [SerializeField] private WorkerListPanel workerList;
    [SerializeField] private Text costText;

    // Дом, который сейчас отображает панель
    private House currentHouse;

    // Storage, на который панель подписана в текущий момент
    private ResourceStorage subscribedStorage;

    private void Awake()
    {
        if (hireButton != null)
            hireButton.onClick.AddListener(OnHireClicked);

        // По умолчанию панель скрыта, пока не выбран дом
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        if (hireButton != null)
            hireButton.onClick.RemoveListener(OnHireClicked);

        Unsubscribe();
    }

    /// <summary>
    /// Показывает панель для указанного дома.
    /// </summary>
    public void Show(House house)
    {
        if (house == null)
            return;

        // Если уже показываем этот же дом — просто обновляем данные
        if (currentHouse == house && gameObject.activeSelf)
        {
            Refresh();
            return;
        }

        Hide();

        currentHouse = house;
        Subscribe();
        gameObject.SetActive(true);

        // Привязываем список worker'ов к текущему дому
        if (workerList != null)
            workerList.Bind(currentHouse);

        Refresh();
    }

    /// <summary>
    /// Полностью скрывает панель и очищает привязку к текущему дому.
    /// </summary>
    public void Hide()
    {
        Unsubscribe();

        currentHouse = null;
        gameObject.SetActive(false);

        if (workerList != null)
            workerList.Bind(null);
    }

    /// <summary>
    /// Подписывается на события дома и storage,
    /// чтобы UI обновлялся автоматически.
    /// </summary>
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

    /// <summary>
    /// Снимает все текущие подписки панели.
    /// </summary>
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

    /// <summary>
    /// Обновляет весь UI панели дома.
    /// </summary>
    private void Refresh()
    {
        if (currentHouse == null)
            return;

        if (limitText != null)
            limitText.text = $"Рабочие: {currentHouse.CurrentWorkers} / {currentHouse.MaxWorkers}";

        ResourceStorage storage = subscribedStorage ?? ResourceStorage.Instance;
        int currentWood = storage != null ? storage.Wood : 0;
        int currentGold = storage != null ? storage.Gold : 0;

        if (costText != null)
        {
            costText.text =
                $"Стоимость найма\n" +
                $"Дерево: {currentWood} / {currentHouse.CurrentWoodCost}\n" +
                $"Золото: {currentGold} / {currentHouse.CurrentGoldCost}";
        }

        // Кнопка активна только если дом действительно может нанять worker'а
        if (hireButton != null)
            hireButton.interactable = currentHouse.CanHire();

        if (workerList != null)
            workerList.Refresh();
    }

    /// <summary>
    /// Обработчик кнопки найма нового worker'а.
    /// </summary>
    public void OnHireClicked()
    {
        if (currentHouse == null)
            return;

        currentHouse.HireWorker();
        Refresh();
    }
}
