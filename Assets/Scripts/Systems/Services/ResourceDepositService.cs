using UnityEngine;
using Zenject;

/// <summary>
/// —ервис сдачи ресурсов в общее хранилище
/// </summary>
public class ResourceDepositService : MonoBehaviour
{
    public static ResourceDepositService Instance { get; private set; }

    private ResourceStorage resourceStorage;

    [Inject]
    private void Construct(ResourceStorage resourceStorage)
    {
        this.resourceStorage = resourceStorage;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Deposit(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
            return;

        resourceStorage.AddResource(resourceType, amount);
    }
}
