using System;
using UnityEngine;

/// <summary>
/// √лобальное хранилище ресурсов игрока
/// уведомл€ет подписчиков при изменении значений
/// </summary>
public class ResourceStorage : MonoBehaviour
{
    public static ResourceStorage Instance { get; private set; }

    public event Action OnResourcesChanged;

    [Header("Current Resources")]
    [SerializeField] private int wood;
    [SerializeField] private int gold;
    [SerializeField] private int meat;

    public int Wood => wood;
    public int Gold => gold;
    public int Meat => meat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        wood = Mathf.Max(0, wood);
        gold = Mathf.Max(0, gold);
        meat = Mathf.Max(0, meat);
    }

    private void Start()
    {
        OnResourcesChanged?.Invoke();
    }

    public void AddWood(int amount)
    {
        AddResource(ResourceType.Wood, amount);
    }

    public void AddGold(int amount)
    {
        AddResource(ResourceType.Gold, amount);
    }

    public void AddMeat(int amount)
    {
        AddResource(ResourceType.Meat, amount);
    }

    /// <summary>
    /// ”ниверсальное добавление ресурса по его типу
    /// </summary>
    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        switch (type)
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

            default:
                return;
        }

        OnResourcesChanged?.Invoke();
    }

    /// <summary>
    /// ¬озвращает текущее количество ресурса указанного типа
    /// </summary>
    public int GetAmount(ResourceType type)
    {
        return type switch
        {
            ResourceType.Wood => wood,
            ResourceType.Gold => gold,
            ResourceType.Meat => meat,
            _ => 0
        };
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

        OnResourcesChanged?.Invoke();
        return true;
    }

    public void SetResources(int newWood, int newGold, int newMeat)
    {
        wood = Mathf.Max(0, newWood);
        gold = Mathf.Max(0, newGold);
        meat = Mathf.Max(0, newMeat);

        OnResourcesChanged?.Invoke();
    }
}

