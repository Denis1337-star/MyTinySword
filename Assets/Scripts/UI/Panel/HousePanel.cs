using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HousePanel : MonoBehaviour
{
    [SerializeField] private Text limitText;
    [SerializeField] private Button hireButton;
    [SerializeField] private WorkerListPanel workerList;
    [SerializeField] private Text costText;

    private House currentHouse;

    private void Awake()
    {
        hireButton.onClick.AddListener(OnHireClicked);
        gameObject.SetActive(false);
    }

    public void Show(House house)
    {
        if (house == null)
        {
            Debug.LogError("HousePanel.Show received null house!");
            return;
        }
        currentHouse = house;
        gameObject.SetActive(true);

        currentHouse.OnWorkersChanged += Refresh;
        ResourceStorage.Instance.OnResourcesChanged += Refresh;

        Refresh();
    }

    public void Hide()
    {
        if (currentHouse != null)
        {
            currentHouse.OnWorkersChanged -= Refresh;
            ResourceStorage.Instance.OnResourcesChanged -= Refresh;
        }

        currentHouse = null;
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (currentHouse != null)
        {
            currentHouse.OnWorkersChanged -= Refresh;
            ResourceStorage.Instance.OnResourcesChanged -= Refresh;
        }
    }


    private void Refresh()
    {
        if (currentHouse == null)
            return; // панель закрыта или не назначена

        limitText.text = $"Нанято {currentHouse.CurrentWorkers} / {currentHouse.MaxWorkers}";
        hireButton.interactable = currentHouse.CanHire();
        costText.text = $"Для найма - Дерево: {currentHouse.CurrentWoodCost} / Золото: {currentHouse.CurrentGoldCost}"; 
        workerList.Refresh();
    }

    public void OnHireClicked()
    {
        if (currentHouse == null)
            return;

        currentHouse.HireWorker();
    }
}
