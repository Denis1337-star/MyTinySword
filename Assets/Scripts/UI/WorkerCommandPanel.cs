using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerCommandPanel : MonoBehaviour
{
    [SerializeField] private Button chopWoodButton;
    [SerializeField] private Button mineGoldButton;

    private Worker currentWorker;

    private void Awake()
    {
        chopWoodButton.onClick.AddListener(OnChopWoodClicked);
        mineGoldButton.onClick.AddListener(OnMineGoldClicked);
        Hide();
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
    }

    public void OnChopWoodClicked()
    {
        Debug.Log("Chop button clicked");
        if (currentWorker == null)
            return;

        currentWorker.AssignChopWoodTask();
        Hide(); 
    }
    public void OnMineGoldClicked()
    {
        if (currentWorker == null)
            return;

        currentWorker.AssignMineGoldTask();
        Hide();
    }
}
