using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

/// <summary>
/// Базовый класс производственного здания
/// </summary>
public abstract class ProductionBuildingBase : BuildingBase
{
    private const float GoldenAngleDegrees = 137.508f;

    [SerializeField] private UnitConfig _unitConfig;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _maxQueue = 1;
    [SerializeField] private float _spawnRadius = 0.8f;
    [SerializeField] private float _spawnNavMeshSearchRadius = 1.5f;

    private ResourceStorage _resourceStorage;
    private ArmyUnitRegistry _armyUnitRegistry;
    private ArmyUnitFactory _armyUnitFactory;
    private TechTreeBonusService _techTreeBonusService;

    private bool _isProducing;
    private Coroutine _productionRoutine;
    private int _queueCount;
    private int _spawnedUnitsCount;

    public UnitConfig UnitConfig => _unitConfig;
    public int CurrentWoodCost => GetReducedCost(_unitConfig.WoodCost);
    public int CurrentMeatCost => GetReducedCost(_unitConfig.MeatCost);
    public float CurrentBuildTime => GetCurrentBuildTime();
    public int QueueCount => _queueCount;
    public int MaxQueue => _maxQueue + GetQueueBonus();
    public bool IsProducing => _isProducing;

    public event Action OnQueueChanged;

    [Inject]
    private void Construct(
        ResourceStorage resourceStorage,
        ArmyUnitRegistry armyUnitRegistry,
        ArmyUnitFactory armyUnitFactory,
        TechTreeBonusService techTreeBonusService)
    {
        _resourceStorage = resourceStorage;
        _armyUnitRegistry = armyUnitRegistry;
        _armyUnitFactory = armyUnitFactory;
        _techTreeBonusService = techTreeBonusService;
    }
    protected override void OnValidate()
    {
        base.OnValidate();

        _maxQueue = Mathf.Max(1, _maxQueue);
        _spawnRadius = Mathf.Max(0f, _spawnRadius);
        _spawnNavMeshSearchRadius = Mathf.Max(0.1f, _spawnNavMeshSearchRadius);
    }

    protected override bool ValidateInternal()
    {
        bool valid = base.ValidateInternal();

        valid &= ValidationUtility.IsValidConfig(this, _unitConfig, nameof(_unitConfig));
        valid &= ValidationUtility.IsAssigned(this, _spawnPoint, nameof(_spawnPoint));

        return valid;
    }

    public bool CanEnqueue()
    {
        return string.IsNullOrEmpty(GetHireBlockReason());
    }

    public string GetHireBlockReason()
    {
        if (_queueCount >= MaxQueue)
            return "Очередь заполнена";

        if (!_armyUnitRegistry.HasFreePlayerSlot())
            return "Достигнут лимит армии";

        if (!_resourceStorage.HasResources(CurrentWoodCost, 0, CurrentMeatCost))
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

        bool spent = _resourceStorage.TrySpendResources(
            CurrentWoodCost,
            0,
            CurrentMeatCost);

        if (!spent)
        {
            _armyUnitRegistry.ReleasePlayerSlotReservation();
            return false;
        }

        _queueCount++;
        OnQueueChanged?.Invoke();

        if (!_isProducing)
            _productionRoutine = StartCoroutine(ProcessQueue());

        return true;
    }

    private IEnumerator ProcessQueue()
    {
        _isProducing = true;

        while (_queueCount > 0)
        {
            yield return new WaitForSeconds(GetCurrentBuildTime());

            bool spawned = SpawnUnit(_unitConfig);

            if (!spawned)
                _armyUnitRegistry.ReleasePlayerSlotReservation();

            _queueCount--;
            OnQueueChanged?.Invoke();
        }

        _isProducing = false;
        _productionRoutine = null;

        OnQueueChanged?.Invoke();
    }

    protected virtual bool SpawnUnit(UnitConfig unitConfig)
    {
        if (unitConfig.Prefab == null)
            return false;

        Vector3 spawnPosition = GetNextSpawnPosition();

        GameObject spawnedObject = _armyUnitFactory.Create(unitConfig.Prefab,
            spawnPosition, Quaternion.identity);

        if (spawnedObject == null)
            return false;

        ArmyUnit armyUnit = spawnedObject.GetComponent<ArmyUnit>();

        if (armyUnit == null)
        {
            Debug.LogError($"{name}: созданный prefab не содержит ArmyUnit.", spawnedObject);
            Destroy(spawnedObject);
            return false;
        }

        if (!armyUnit.IsPlayerUnit())
        {
            Debug.LogWarning(
                $"{name}: здание производства создало юнита не с Player-фракцией. Проверь Faction на prefab ArmyUnit.",
                spawnedObject);

            Destroy(spawnedObject);
            return false;
        }

        return true;
    }

    private Vector3 GetNextSpawnPosition()
    {
        Vector3 center = _spawnPoint.position;

        int index = _spawnedUnitsCount++;

        float angle = index * GoldenAngleDegrees * Mathf.Deg2Rad;
        float radius = _spawnRadius * Mathf.Sqrt(index + 1f) * 0.5f;

        Vector3 candidate = center + new Vector3(
            Mathf.Cos(angle), Mathf.Sin(angle),0f) * radius;

        if (NavMesh.SamplePosition(candidate,out NavMeshHit hit,
                _spawnNavMeshSearchRadius,NavMesh.AllAreas))
        {
            return hit.position;
        }

        return center;
    }

    private int GetReducedCost(int baseCost)
    {
        if (baseCost <= 0)
            return 0;

        float reducedCost = _techTreeBonusService.ApplyPercentReduction(
            baseCost,
            TechTreeBonusType.HireArmy);

        return Mathf.Max(0, Mathf.CeilToInt(reducedCost));
    }
    private float GetCurrentBuildTime()
    {
        float buildTime = _techTreeBonusService.ApplyPercentReduction(
            _unitConfig.BuildTime,
            TechTreeBonusType.TrainArmy);

        return Mathf.Max(0.1f, buildTime);
    }
    private int GetQueueBonus()
    {
        return _techTreeBonusService.GetBonusInt(TechTreeBonusType.QueueMilitaryBuildings);
    }

    protected virtual void OnDestroy()
    {
        if (_productionRoutine != null)
            StopCoroutine(_productionRoutine);

        _armyUnitRegistry.ReleasePlayerSlotReservations(_queueCount);

        _queueCount = 0;

        OnQueueChanged?.Invoke();
    }
}