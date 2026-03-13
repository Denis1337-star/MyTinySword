using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerInventory : MonoBehaviour
{
    public int CarriedAmount { get; private set; }

    public bool HasCargo => CarriedAmount > 0;

    public void SetCargo(int amount)
    {
        CarriedAmount = amount;
    }

    public int TakeCargo()
    {
        int amount = CarriedAmount;
        CarriedAmount = 0;
        return amount;
    }

    public void Clear()
    {
        CarriedAmount = 0;
    }
}
