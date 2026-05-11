/// <summary>
/// Инвентарь worker для переносимого ресурса
/// </summary>
public sealed class WorkerInventory
{
    private ResourceType _resourceType = ResourceType.None;
    private int _amount;

    public bool HasCargo => _amount > 0 && _resourceType != ResourceType.None;
    public ResourceType CarriedResourceType => _resourceType;
    public int Amount => _amount;

    public void SetCargo(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None || amount <= 0)
        {
            Clear();
            return;
        }

        _resourceType = resourceType;
        _amount = amount;
    }

    public int TakeCargo(out ResourceType resourceType)
    {
        if (!HasCargo)
        {
            resourceType = ResourceType.None;
            return 0;
        }

        resourceType = _resourceType;
        int amount = _amount;

        Clear();

        return amount;
    }

    public void Clear()
    {
        _resourceType = ResourceType.None;
        _amount = 0;
    }
}