using UnityEngine;

/// <summary>
///  инвентарь worker
/// </summary>
public class WorkerInventory : MonoBehaviour
{
    private ResourceType _resourceType = ResourceType.None;
    private int _carriedAmount;

    public ResourceType CarriedResourceType => _resourceType;
    public int CarriedAmount => _carriedAmount;
    public bool HasCargo => _resourceType != ResourceType.None && _carriedAmount > 0;

    public void SetCargo(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None || amount <= 0)
        {
            Clear();
            return;
        }

        _resourceType = resourceType;
        _carriedAmount = amount;
    }

    public int TakeCargo(out ResourceType resourceType)
    {
        resourceType = _resourceType;

        int amount = _carriedAmount;

        Clear();

        return amount;
    }

    public void Clear()
    {
        _resourceType = ResourceType.None;
        _carriedAmount = 0;
    }
}
