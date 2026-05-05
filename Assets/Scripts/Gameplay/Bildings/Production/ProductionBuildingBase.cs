using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Базовый класс здания найма.
/// Хранит очередь производства, списывает ресурсы и создаёт боевого юнита.
/// </summary>
public class ProductionBuildingBase : BuildingBase
{
    [Header("Production")]
    [SerializeField] private UnitConfig _unitConfig;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _maxQueue = 1;

    [Header("Spawn Spread")]
    [SerializeField] private float _spawnRadius = 0.8f;
    [SerializeField] private float _spawnNavMeshSearchRadius = 1.5f;

    private readonly Queue<UnitConfig> _productionQueue = new();

    private ResourceStorage _resourceStorage;
    private ArmyUnitRegistry _armyUnitRegistry;
    private ArmyUnitFactory _armyUnitFactory;

    private bool _isProducing;
    private Coroutine _productionRoutine;
    private int _spawnedUnitsCount;

    public UnitConfig UnitConfig => _unitConfig;
    public int QueueCount => _productionQueue.Count;
    public int MaxQueue => _maxQueue;
    public bool IsProducing => _isProducing;

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
        _spawnRadius = Mathf.Max(0f, _spawnRadius);
        _spawnNavMeshSearchRadius = Mathf.Max(0.1f, _spawnNavMeshSearchRadius);
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

        if (_resourceStorage == null)
            return "Хранилище ресурсов не найдено";

        if (_armyUnitRegistry == null)
            return "Реестр армии не найден";

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

        if (!_armyUnitRegistry.TryReservePlayerSlot())
            return false;

        bool spent = _resourceStorage.TrySpendUnitResources(
            _unitConfig.WoodCost,
            _unitConfig.MeatCost);

        if (!spent)
        {
            _armyUnitRegistry.ReleasePlayerSlotReservation();
            return false;
        }

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

                _armyUnitRegistry?.ReleasePlayerSlotReservation();

                OnQueueChanged?.Invoke();
                continue;
            }

            yield return new WaitForSeconds(unitConfig.BuildTime);

            bool spawned = SpawnUnit(unitConfig);

            if (!spawned)
                _armyUnitRegistry?.ReleasePlayerSlotReservation();

            _productionQueue.Dequeue();
            OnQueueChanged?.Invoke();
        }

        _isProducing = false;
        _productionRoutine = null;

        OnQueueChanged?.Invoke();
    }

    protected virtual bool SpawnUnit(UnitConfig unitConfig)
    {
        if (unitConfig == null || unitConfig.Prefab == null)
            return false;

        if (_armyUnitFactory == null)
        {
            Debug.LogError($"{name}: ArmyUnitFactory не внедрён через Zenject.", this);
            return false;
        }

        Vector3 spawnPosition = GetNextSpawnPosition();

        GameObject spawnedObject = _armyUnitFactory.Create(
            unitConfig.Prefab,
            spawnPosition,
            Quaternion.identity);

        if (spawnedObject == null)
            return false;

        ArmyUnit armyUnit = spawnedObject.GetComponent<ArmyUnit>();
        if (armyUnit == null)
            armyUnit = spawnedObject.GetComponentInChildren<ArmyUnit>();

        if (armyUnit == null)
        {
            Debug.LogError($"{name}: созданный prefab не содержит ArmyUnit.", spawnedObject);
            return false;
        }

        if (!armyUnit.IsPlayerUnit())
        {
            Debug.LogWarning(
                $"{name}: здание производства создало юнита не с Player-фракцией. Проверь FactionMember на prefab.",
                spawnedObject);

            return false;
        }

        return true;
    }

    private Vector3 GetNextSpawnPosition()
    {
        Vector3 center = _spawnPoint != null
            ? _spawnPoint.position
            : transform.position;

        const float goldenAngle = 137.508f;

        int index = _spawnedUnitsCount++;

        float angle = index * goldenAngle * Mathf.Deg2Rad;
        float radius = _spawnRadius * Mathf.Sqrt(index + 1f) * 0.5f;

        Vector3 candidate = center + new Vector3(
            Mathf.Cos(angle),
            Mathf.Sin(angle),
            0f) * radius;

        if (UnityEngine.AI.NavMesh.SamplePosition(
                candidate,
                out UnityEngine.AI.NavMeshHit hit,
                _spawnNavMeshSearchRadius,
                UnityEngine.AI.NavMesh.AllAreas))
        {
            return hit.position;
        }

        return center;
    }

    private void OnDestroy()
    {
        if (_productionRoutine != null)
            StopCoroutine(_productionRoutine);

        int reservedSlotsToRelease = _productionQueue.Count;

        _productionQueue.Clear();

        _armyUnitRegistry?.ReleasePlayerSlotReservations(reservedSlotsToRelease);

        OnQueueChanged?.Invoke();
    }
}