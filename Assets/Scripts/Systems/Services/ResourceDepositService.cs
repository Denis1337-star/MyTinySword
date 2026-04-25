using UnityEngine;

/// <summary>
/// —ервис сдачи ресурсов в общее хранилище
/// </summary>
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

    /// <summary>
    /// —дает ресурс указанного типа в общее хранилище
    /// </summary>
    public void Deposit(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
            return;

        if (ResourceStorage.Instance == null)
            return;

        ResourceStorage.Instance.AddResource(resourceType, amount);
    }
}
