using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Базовый класс производственного здания.
/// Умеет хранить доступных юнитов, очередь найма и спавнить готовый prefab.
/// </summary>
public class ProductionBuildingBase : BuildingBase
{
    [Header("Production")]
    [SerializeField] private List<UnitConfig> availableUnits = new();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxQueue = 8;

    private readonly Queue<UnitConfig> productionQueue = new();
    private bool isProducing;

    public IReadOnlyList<UnitConfig> AvailableUnits => availableUnits;
    public int QueueCount => productionQueue.Count;
    public bool IsProducing => isProducing;

    public event Action OnQueueChanged;

    private void OnValidate()
    {
        if (spawnPoint == null)
        {
            Transform child = transform.Find("SpawnPoint");
            if (child != null)
                spawnPoint = child;
        }
    }

    /// <summary>
    /// Можно ли добавить юнита в очередь.
    /// </summary>
    public bool CanEnqueue(UnitConfig config)
    {
        if (config == null)
            return false;

        if (!availableUnits.Contains(config))
            return false;

        if (productionQueue.Count >= maxQueue)
            return false;

        if (ResourceStorage.Instance == null)
            return false;

        if (!ResourceStorage.Instance.HasResources(config.WoodCost, config.GoldCost))
            return false;

        if (ArmyUnitRegistry.Instance == null)
            return false;

        if (!ArmyUnitRegistry.Instance.HasFreePlayerSlot())
            return false;

        return true;
    }

    /// <summary>
    /// Пытается добавить юнита в очередь найма.
    /// </summary>
    public bool TryEnqueue(UnitConfig config)
    {
        if (!CanEnqueue(config))
            return false;

        if (ResourceStorage.Instance == null)
            return false;

        bool spent = ResourceStorage.Instance.TrySpendResources(config.WoodCost, config.GoldCost);
        if (!spent)
            return false;

        productionQueue.Enqueue(config);
        OnQueueChanged?.Invoke();

        if (!isProducing)
            StartCoroutine(ProcessQueue());

        return true;
    }

    /// <summary>
    /// Последовательно обрабатывает очередь производства.
    /// </summary>
    private IEnumerator ProcessQueue()
    {
        isProducing = true;

        while (productionQueue.Count > 0)
        {
            UnitConfig config = productionQueue.Peek();

            if (config == null)
            {
                productionQueue.Dequeue();
                OnQueueChanged?.Invoke();
                continue;
            }

            yield return new WaitForSeconds(config.BuildTime);

            SpawnUnit(config);
            productionQueue.Dequeue();
            OnQueueChanged?.Invoke();
        }

        isProducing = false;
    }

    /// <summary>
    /// Создаёт готового юнита в точке спавна.
    /// </summary>
    protected virtual void SpawnUnit(UnitConfig config)
    {
        if (config == null || config.Prefab == null)
            return;

        Vector3 spawnPosition = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        Instantiate(config.Prefab, spawnPosition, Quaternion.identity);
    }
}