using System;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    public static ResourceStorage Instance { get; private set; }

    public event Action OnResourcesChanged;

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
        if (amount <= 0)
            return;

        wood += amount;
        OnResourcesChanged?.Invoke();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        gold += amount;
        OnResourcesChanged?.Invoke();
    }

    public void AddMeat(int amount)
    {
        if (amount <= 0)
            return;

        meat += amount;
        OnResourcesChanged?.Invoke();
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

