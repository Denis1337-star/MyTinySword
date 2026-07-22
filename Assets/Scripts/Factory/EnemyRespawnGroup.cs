using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyRespawnGroup : ValidatedMonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoint;
    [SerializeField] private int _count = 2;

    [Header("Respawn")]
    [SerializeField] private float _respawnDelay = 45f;
    [SerializeField] private bool _respawnWhenAllDead = true;
    [SerializeField] private bool _spawnOnStart = true;

    private ArmyUnitFactory _armyUnitFactory;
    private readonly List<ArmyUnit> _aliveUnit = new();
    private Coroutine _respawnRoutine;
    private bool _isRespawning;

    [Inject]
    private void Construct(ArmyUnitFactory armyUnitFactory)
    {
        _armyUnitFactory = armyUnitFactory;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        if (_spawnOnStart)
            SpawnGroup();
        
    }
    private void OnDisable()
    {
        if (_respawnRoutine != null)
        {
            StopCoroutine(_respawnRoutine);
            _respawnRoutine = null;
        }
        _isRespawning = false;
    }
    protected override bool ValidateInternal()
    {
        bool valid = true;
        valid &= ValidationUtility.IsAssigned(this, _enemyPrefab, nameof(_enemyPrefab));
        return valid;
    }
    public void SpawnGroup()
    {
        ClearDeadReferences();

        int toSpawn = Mathf.Max(1,_count);

        for (int i = 0; i < toSpawn; i++)
        {
            Transform point = GetSpawnPoint(i);
            if(point==null)
                continue;

            GameObject instance = _armyUnitFactory.Create(_enemyPrefab, point.position,
                point.rotation);

            if (instance==null)
                continue;

            ArmyUnit unit = instance.GetComponent<ArmyUnit>();
          
            _aliveUnit.Add(unit);
            unit.Health.OnDied += HandleUnitDied;
            instance.transform.SetParent(transform, true);
        }
    }
    private void HandleUnitDied()
    {
        ClearDeadReferences();

        if (!_respawnWhenAllDead)
            return;

        if(_aliveUnit.Count>0)
            return;

        if(_isRespawning) return;

        _respawnRoutine = StartCoroutine(RespawnAfterDelay());
    }
    private IEnumerator RespawnAfterDelay()
    {
        _isRespawning = true;

        yield return new WaitForSeconds(_respawnDelay);

        _isRespawning = false;
        _respawnRoutine = null;
        SpawnGroup();
    }
    private void ClearDeadReferences()
    {
        for (int i = _aliveUnit.Count - 1; i >= 0; i--)
        {
            ArmyUnit unit = _aliveUnit[i];
            if (unit == null || unit.IsDead)
            {
                if (unit != null)
                    unit.Health.OnDied -=HandleUnitDied;

                _aliveUnit.RemoveAt(i);
            }
        }
    }
    private Transform GetSpawnPoint(int index)
    {
        return _spawnPoint[index %_spawnPoint.Length];
    }
}
