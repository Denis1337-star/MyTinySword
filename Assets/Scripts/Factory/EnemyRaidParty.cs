using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaidParty : ValidatedMonoBehaviour
{
    [SerializeField] private List<ArmyUnit> _raidUnits = new();
    [SerializeField] private Transform _unitsRoot;
    [SerializeField] private Transform _raidTarget;

    [SerializeField] private float _firstRaidDelay = 90f;
    [SerializeField] private float _raidInterval = 90f;
    [SerializeField] private int _unitsPerRaid = 3;

    private Coroutine _raidRoutine;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        if (!enabled)
            return;

        _raidRoutine = StartCoroutine(RaidLoop());
    }
    private void OnDisable()
    {
        if (_raidRoutine != null)
        {
            StopCoroutine(_raidRoutine);
            _raidRoutine = null;
        }
    }
    protected override bool ValidateInternal()
    {
        bool valid = true;
        valid &= ValidationUtility.IsAssigned(this, _raidTarget, nameof(_raidTarget));
        return valid;
    }

    private IEnumerator RaidLoop()
    {
        if (_firstRaidDelay > 0f)
            yield return new WaitForSeconds(_firstRaidDelay);

        while (true)
        {
            LaunchRaid();
            yield return new WaitForSeconds(_raidInterval);
        }
    }
    private void LaunchRaid()
    {
        if (_raidTarget == null)
            return;

        List<ArmyUnit> candidates = CollectAAliveUnits();
        if (candidates.Count == 0)
            return;

        int count = Mathf.Min(_unitsPerRaid, candidates.Count);

        for (int i = 0; i < count; i++)
        {
            ArmyUnit unit = candidates[i];
            if (unit == null || unit.IsDead || unit.Brain == null)
                continue;

            unit.Brain.MoveTo(_raidTarget.position);
        }
    }
    private List<ArmyUnit> CollectAAliveUnits()
    {
        var result = new List<ArmyUnit>();

        for (int i = 0; i < _raidUnits.Count; i++)
        {
            ArmyUnit unit = _raidUnits[i];
            if (unit != null && !unit.IsDead)
                result.Add(unit);
        }

        if (_unitsRoot != null)
        {
            ArmyUnit[] fromRoot = _unitsRoot.GetComponentsInChildren<ArmyUnit>(true);
            for (int i = 0; i < fromRoot.Length; i++)
            {
                ArmyUnit unit = fromRoot[i];
                if (unit == null || unit.IsDead)
                    continue;

                if (!result.Contains(unit))
                    result.Add(unit);
            }
        }
        return result;
    }
}
