using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Базовый класс  здания найма
///  Хранит очередь производства, списывает ресурсы и создаёт боевого юнита
/// </summary>
public class ProductionBuildingBase : BuildingBase
{
    [SerializeField] private UnitConfig _unitConfig;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _maxQueue = 1;

    private readonly Queue<UnitConfig> _productionQueue = new();

    private ResourceStorage _resourceStorage;
    private ArmyUnitRegistry _armyUnitRegistry;
    private ArmyUnitFactory _armyUnitFactory;

    private bool _isProducing;
    private Coroutine _productionRoutine;

    public UnitConfig UnitConfig => _unitConfig;
    public int QueueCount => _productionQueue.Count;
    public bool IsProducing => _isProducing;

    public int WoodCost =>  _unitConfig.WoodCost;
    public int MeatCost => _unitConfig.MeatCost;

    public event Action OnQueueChanged;

    [Inject]
    private void Construct(
        ResourceStorage resourceStorage,
        ArmyUnitRegistry armyUnitRegistry,
    ArmyUnitFactory armyUnitFactory)
    {
        _resourceStorage = resourceStorage;
        _armyUnitRegistry = armyUnitRegistry;
        _armyUnitFactory = armyUnitFactory;
    }

    private void OnValidate()
    {
        _maxQueue = Mathf.Max(1, _maxQueue);
    }

    public bool CanEnqueue()
    {
        return string.IsNullOrEmpty(GetHireBlockReason());
    }

    public string GetHireBlockReason()
    {
        if (_unitConfig == null)
            return "Юнит не назначен";

        if (_unitConfig.Prefab == null)
            return "Prefab юнита не назначен";

        if (_spawnPoint == null)
            return "Точка спавна не назначена";

        if (_productionQueue.Count >= _maxQueue)
            return "Очередь заполнена";

        if (!_armyUnitRegistry.HasFreePlayerSlot())
            return "Достигнут лимит армии";

        if (!_resourceStorage.HasUnitResources(_unitConfig.WoodCost, _unitConfig.MeatCost))
            return "Не хватает ресурсов";

        return string.Empty;
    }

    public bool TryHireUnit()
    {
        return TryEnqueue();
    }

    public bool TryEnqueue()
    {
        if (!CanEnqueue())
            return false;

        bool spent = _resourceStorage.TrySpendUnitResources(
            _unitConfig.WoodCost,
            _unitConfig.MeatCost);

        if (!spent)
            return false;

        _productionQueue.Enqueue(_unitConfig);
        OnQueueChanged?.Invoke();

        if (!_isProducing)
            _productionRoutine = StartCoroutine(ProcessQueue());

        return true;
    }

    private IEnumerator ProcessQueue()
    {
        _isProducing = true;

        while (_productionQueue.Count > 0)
        {
            UnitConfig unitConfig = _productionQueue.Peek();

            if (unitConfig == null)
            {
                _productionQueue.Dequeue();
                OnQueueChanged?.Invoke();
                continue;
            }

            yield return new WaitForSeconds(unitConfig.BuildTime);

            SpawnUnit(unitConfig);

            _productionQueue.Dequeue();
            OnQueueChanged?.Invoke();
        }

        _isProducing = false;
        _productionRoutine = null;
    }

    protected virtual void SpawnUnit(UnitConfig unitConfig)
    {
        if (unitConfig == null || unitConfig.Prefab == null)
            return;

        Vector3 spawnPosition =  _spawnPoint.position;

        _armyUnitFactory.Create(
            unitConfig.Prefab,
            spawnPosition,
            Quaternion.identity);
    }
}