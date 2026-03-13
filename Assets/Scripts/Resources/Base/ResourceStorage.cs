using System;
using UnityEngine;
using UnityEngine.UI;

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
    }

    private void Start()
    {
        OnResourcesChanged?.Invoke();
    }

    public void AddWood(int amount)
    {
        wood += amount;
        OnResourcesChanged?.Invoke();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnResourcesChanged?.Invoke();
    }

    public void AddMeat(int amount)
    {
        meat += amount;
        OnResourcesChanged?.Invoke();
    }

    public bool HasResources(int requiredWood, int requiredGold)
    {
        return wood >= requiredWood && gold >= requiredGold;
    }

    public void SpendResources(int spendWood, int spendGold)
    {
        wood -= spendWood;
        gold -= spendGold;
        OnResourcesChanged?.Invoke();
    }
}
