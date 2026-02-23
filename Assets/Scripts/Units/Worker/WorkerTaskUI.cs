using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerTaskUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    [SerializeField] private Sprite axeSprite;
    [SerializeField] private Sprite woodSprite;

    private Worker worker;

    private void Update()
    {
        if (worker == null) return;

        switch (worker.CurrentState)
        {
            case WorkerState.Idle:
                iconImage.enabled = false;
                break;

            case WorkerState.GoingToResource:
            case WorkerState.Working:
                iconImage.enabled = true;
                iconImage.sprite = axeSprite;
                break;

            case WorkerState.CarryingToHouse:
                iconImage.enabled = true;
                iconImage.sprite = woodSprite;
                break;
        }
    }

    public void Initialize(Worker workerRef)
    {
        worker = workerRef;
    }
}
