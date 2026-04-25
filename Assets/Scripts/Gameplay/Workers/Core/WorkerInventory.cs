using UnityEngine;

/// <summary>
///  инвентарь worker
/// </summary>
public class WorkerInventory : MonoBehaviour
{
    public ResourceType CarriedResourceType { get; private set; } = ResourceType.None;
    public int CarriedAmount { get; private set; }

    public bool HasCargo => CarriedResourceType != ResourceType.None && CarriedAmount > 0;

    public void SetCargo(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None || amount <= 0)
        {
            Clear();
            return;
        }

        CarriedResourceType = resourceType;
        CarriedAmount = amount;
    }

    public int TakeCargo(out ResourceType resourceType)
    {
        resourceType = CarriedResourceType;

        int amount = CarriedAmount;
        Clear();

        return amount;
    }

    public void Clear()
    {
        CarriedResourceType = ResourceType.None;
        CarriedAmount = 0;
    }
}
