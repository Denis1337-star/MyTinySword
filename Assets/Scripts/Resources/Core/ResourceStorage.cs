using System;
using UnityEngine;

/// <summary>
/// √лобальное хранилище ресурсов игрока
/// ’ранит текущее количество дерева, золота и м€са
/// а также уведомл€ет подписчиков при изменении значений
/// </summary>
public class ResourceStorage : MonoBehaviour
{
    public static ResourceStorage Instance { get; private set; }   // √лобальна€ точка доступа к общему хранилищу ресурсов

    public event Action OnResourcesChanged;   // —обытие вызываетс€ каждый раз, когда количество ресурсов изменилось

    [Header("Current Resources")]
    [SerializeField] private int wood;
    [SerializeField] private int gold;
    [SerializeField] private int meat;

    // ѕубличный доступ только на чтение
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

    /// <summary>
    /// ѕровер€ет, хватает ли дерева и золота на действие
    /// </summary>
    public bool HasResources(int requiredWood, int requiredGold)
    {
        requiredWood = Mathf.Max(0, requiredWood);
        requiredGold = Mathf.Max(0, requiredGold);

        return wood >= requiredWood && gold >= requiredGold;
    }

    /// <summary>
    /// ѕытаетс€ списать дерево и золото
    /// </summary>
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

    /// <summary>
    /// ѕолностью устанавливает новые значени€ ресурсов
    /// ѕолезно дл€ отладки, тестов или загрузки сохранений
    /// </summary>
    public void SetResources(int newWood, int newGold, int newMeat)
    {
        wood = Mathf.Max(0, newWood);
        gold = Mathf.Max(0, newGold);
        meat = Mathf.Max(0, newMeat);
        OnResourcesChanged?.Invoke();
    }
}

