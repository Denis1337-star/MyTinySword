using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Общая UI-панель производственных зданий:
/// Barracks, Archery, Monastery.
/// Показывает данные выбранного юнита и позволяет поставить его в очередь.
/// </summary>
public class ProductionBuildingPanel : MonoBehaviour
{
    [Header("Main Info")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text statsText;
    [SerializeField] private Text costText;
    [SerializeField] private Text queueText;
    [SerializeField] private Image iconImage;

    [Header("Button")]
    [SerializeField] private Button hireButton;
    [SerializeField] private Text hireButtonText;

    private ProductionBuildingBase currentBuilding;
    private UnitConfig selectedUnit;

    private void Awake()
    {
        if (hireButton != null)
            hireButton.onClick.AddListener(OnHireClicked);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (hireButton != null)
            hireButton.onClick.RemoveListener(OnHireClicked);

        Unsubscribe();
    }

    /// <summary>
    /// Показывает панель для выбранного производственного здания.
    /// </summary>
    public void Show(ProductionBuildingBase building)
    {
        if (building == null)
            return;

        if (currentBuilding == building && gameObject.activeSelf)
        {
            Refresh();
            return;
        }

        Unsubscribe();

        currentBuilding = building;
        Subscribe();

        if (currentBuilding.AvailableUnits != null && currentBuilding.AvailableUnits.Count > 0)
            selectedUnit = currentBuilding.AvailableUnits[0];
        else
            selectedUnit = null;

        gameObject.SetActive(true);
        Refresh();
    }

    /// <summary>
    /// Скрывает панель.
    /// </summary>
    public void Hide()
    {
        Unsubscribe();
        currentBuilding = null;
        selectedUnit = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Позже пригодится, если в одном здании будет несколько видов юнитов.
    /// </summary>
    public void SelectUnit(UnitConfig unit)
    {
        selectedUnit = unit;
        Refresh();
    }

    private void Subscribe()
    {
        if (currentBuilding != null)
            currentBuilding.OnQueueChanged += Refresh;
    }

    private void Unsubscribe()
    {
        if (currentBuilding != null)
            currentBuilding.OnQueueChanged -= Refresh;
    }

    private void Refresh()
    {
        if (currentBuilding == null)
            return;

        if (selectedUnit == null)
        {
            if (titleText != null)
                titleText.text = "Нет доступного юнита";

            if (descriptionText != null)
                descriptionText.text = string.Empty;

            if (statsText != null)
                statsText.text = string.Empty;

            if (costText != null)
                costText.text = string.Empty;

            if (queueText != null)
                queueText.text = $"В очереди: {currentBuilding.QueueCount}";

            if (iconImage != null)
                iconImage.sprite = null;

            if (hireButton != null)
                hireButton.interactable = false;

            if (hireButtonText != null)
                hireButtonText.text = "Нанять";

            return;
        }

        if (titleText != null)
            titleText.text = selectedUnit.DisplayName;

        if (descriptionText != null)
            descriptionText.text = selectedUnit.Description;

        if (statsText != null)
        {
            statsText.text =
                $"Урон: {selectedUnit.Damage}\n" +
                $"Здоровье: {selectedUnit.MaxHealth} HP";
        }

        if (costText != null)
        {
            costText.text =
                $"Стоимость: {selectedUnit.GoldCost} золота и {selectedUnit.WoodCost} дерева\n" +
                $"Время найма: {selectedUnit.BuildTime:0.#} секунды";
        }

        if (queueText != null)
            queueText.text = $"В очереди: {currentBuilding.QueueCount}";

        if (iconImage != null)
            iconImage.sprite = selectedUnit.Icon;

        bool canHire = currentBuilding.CanEnqueue(selectedUnit);

        if (hireButton != null)
            hireButton.interactable = canHire;

        if (hireButtonText != null)
            hireButtonText.text = canHire ? "Нанять" : "Не хватает ресурсов";
    }

    private void OnHireClicked()
    {
        if (currentBuilding == null || selectedUnit == null)
            return;

        bool success = currentBuilding.TryEnqueue(selectedUnit);
        if (!success)
            return;

        Refresh();
    }
}