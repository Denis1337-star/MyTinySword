using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerCommandPanel : MonoBehaviour
{
    [SerializeField] private Button chopWoodButton;
    [SerializeField] private Button mineGoldButton;
    [SerializeField] private Button huntMeatButton;
    [SerializeField] private SelectionSystem selectionSystem;

    private Worker currentWorker;

    private void Awake()
    {
        chopWoodButton.onClick.AddListener(OnChopWoodClicked);
        mineGoldButton.onClick.AddListener(OnMineGoldClicked);
        huntMeatButton.onClick.AddListener(OnHuntMeatClicked);
        Hide();
    }

    public void ShowForWorker(Worker worker)
    {
        if (worker == null)
            return;

        currentWorker = worker;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentWorker = null;
        selectionSystem.ClearSelection();
        gameObject.SetActive(false);
        
    }

    public void OnChopWoodClicked()
    {
        if (currentWorker == null)
            return;

        currentWorker.AssignJob(WorkerJobType.ChopWood);
        Hide();
    }

    public void OnMineGoldClicked()
    {
        if (currentWorker == null)
            return;

        currentWorker.AssignJob(WorkerJobType.MineGold);
        Hide();
    }

    public void OnHuntMeatClicked()
    {
        if (currentWorker == null)
            return;

        currentWorker.AssignJob(WorkerJobType.HuntMeat);
        Hide();
    }
}
