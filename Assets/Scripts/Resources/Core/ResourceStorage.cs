using System;
using UnityEngine;

/// <summary>
/// Центральное хранилище ресурсов 
/// </summary>
public sealed class ResourceStorage : MonoBehaviour
{
    [SerializeField, Min(0)] private int _wood;
    [SerializeField, Min(0)] private int _gold;
    [SerializeField, Min(0)] private int _meat;

    public event Action ResourcesChanged;

    public int Wood => _wood;
    public int Gold => _gold;
    public int Meat => _meat;

    /// <summary>
    /// Возвращает текущее количество конкретного ресурса
    /// </summary>
    public int GetAmount(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Wood => _wood,
            ResourceType.Gold => _gold,
            ResourceType.Meat => _meat,
            _ => 0
        };
    }

    /// <summary>
    /// Добавляет ресурс в хранилище
    /// </summary>
    public void AddResource(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
            return;

        bool changed = true;

        switch (resourceType)
        {
            case ResourceType.Wood:
                _wood += amount;
                break;

            case ResourceType.Gold:
                _gold += amount;
                break;

            case ResourceType.Meat:
                _meat += amount;
                break;

            case ResourceType.None:
            default:
                changed = false;
                Debug.LogWarning($"{name}: попытка добавить неизвестный ресурс {resourceType}.", this);
                break;
        }

        if (changed)
            NotifyResourcesChanged();
    }

    /// <summary>
    /// Проверяет хватает ли ресурсов
    /// </summary>
    public bool HasResources(int woodCost, int goldCost, int meatCost)
    {
        if (woodCost < 0 || goldCost < 0 || meatCost < 0)
            return false;

        return _wood >= woodCost &&
               _gold >= goldCost &&
               _meat >= meatCost;
    }

    /// <summary>
    /// списывает ресурсы
    /// </summary>
    public bool TrySpendResources(int woodCost, int goldCost, int meatCost)
    {
        if (!HasResources(woodCost, goldCost, meatCost))
            return false;

        _wood -= woodCost;
        _gold -= goldCost;
        _meat -= meatCost;

        NotifyResourcesChanged();
        return true;
    }

    private void NotifyResourcesChanged()
    {
        ResourcesChanged?.Invoke();
    }
}