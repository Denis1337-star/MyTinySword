using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepositService : MonoBehaviour
{
    public static ResourceDepositService Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Deposit(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
            return;

        if (ResourceStorage.Instance == null)
            return;

        switch (resourceType)
        {
            case ResourceType.Wood:
                ResourceStorage.Instance.AddWood(amount);
                break;

            case ResourceType.Gold:
                ResourceStorage.Instance.AddGold(amount);
                break;

            case ResourceType.Meat:
                ResourceStorage.Instance.AddMeat(amount);
                break;
        }
    }
}
