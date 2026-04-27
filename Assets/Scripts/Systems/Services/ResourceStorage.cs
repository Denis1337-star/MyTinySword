using System;
using UnityEngine;
using UniRx;

/// <summary>
/// Глобальное хранилище ресурсов игрока
/// </summary>
public class ResourceStorage : MonoBehaviour
{
    public static ResourceStorage Instance { get; private set; }

    [Header("Current Resources")]
    [SerializeField] private int wood;
    [SerializeField] private int gold;
    [SerializeField] private int meat;

    private readonly Subject<Unit> resourcesChanged = new();

    public int Wood => wood;
    public int Gold => gold;
    public int Meat => meat;

    public event Action OnResourcesChanged;

    public IObservable<Unit> ResourcesChanged => resourcesChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ClampResources();
    }

    private void Start()
    {
        NotifyResourcesChanged();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        resourcesChanged.OnCompleted();
        resourcesChanged.Dispose();
    }

    public void AddResource(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
            return;

        switch (resourceType)
        {
            case ResourceType.Wood:
                wood += amount;
                break;

            case ResourceType.Gold:
                gold += amount;
                break;

            case ResourceType.Meat:
                meat += amount;
                break;

            case ResourceType.None:
            default:
                return;
        }

        NotifyResourcesChanged();
    }

    public bool HasResources(int requiredWood, int requiredGold)
    {
        requiredWood = Mathf.Max(0, requiredWood);
        requiredGold = Mathf.Max(0, requiredGold);

        return wood >= requiredWood && gold >= requiredGold;
    }

    public bool TrySpendResources(int spendWood, int spendGold)
    {
        spendWood = Mathf.Max(0, spendWood);
        spendGold = Mathf.Max(0, spendGold);

        if (!HasResources(spendWood, spendGold))
            return false;

        wood -= spendWood;
        gold -= spendGold;

        NotifyResourcesChanged();
        return true;
    }

    public bool HasUnitResources(int requiredWood, int requiredMeat)
    {
        requiredWood = Mathf.Max(0, requiredWood);
        requiredMeat = Mathf.Max(0, requiredMeat);

        return wood >= requiredWood && meat >= requiredMeat;
    }

    public bool TrySpendUnitResources(int spendWood, int spendMeat)
    {
        spendWood = Mathf.Max(0, spendWood);
        spendMeat = Mathf.Max(0, spendMeat);

        if (!HasUnitResources(spendWood, spendMeat))
            return false;

        wood -= spendWood;
        meat -= spendMeat;

        NotifyResourcesChanged();
        return true;
    }

    public void SetResources(int newWood, int newGold, int newMeat)
    {
        wood = Mathf.Max(0, newWood);
        gold = Mathf.Max(0, newGold);
        meat = Mathf.Max(0, newMeat);

        NotifyResourcesChanged();
    }

    private void OnValidate()
    {
        ClampResources();
    }

    private void ClampResources()
    {
        wood = Mathf.Max(0, wood);
        gold = Mathf.Max(0, gold);
        meat = Mathf.Max(0, meat);
    }

    private void NotifyResourcesChanged()
    {
        OnResourcesChanged?.Invoke();
        resourcesChanged.OnNext(Unit.Default);
    }
}

