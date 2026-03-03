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
        huntMeatButton.onClick.AddListener (OnHuntMeatClicked);
        Hide();
    }
    private void OnDisable()
    {
        selectionSystem.ClearSelection();
    }

    public void ShowForWorker(Worker worker)
    {
        currentWorker = worker;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentWorker = null;
        gameObject.SetActive(false);
        selectionSystem.ClearSelection();
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
        currentWorker?.AssignJob(WorkerJobType.HuntMeat);
        Hide();
    }
}
