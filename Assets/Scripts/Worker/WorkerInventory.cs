using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerInventory : MonoBehaviour
{
    public int CarriedAmount { get; private set; }

    public void SetAmount(int amount)
    {
        CarriedAmount = amount;
    }

    public int TakeAll()
    {
        int amount = CarriedAmount;
        CarriedAmount = 0;
        return amount;
    }

    public bool HasResources()
    {
        return CarriedAmount > 0;
    }
}
