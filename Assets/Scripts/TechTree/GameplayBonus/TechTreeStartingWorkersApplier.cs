using UnityEngine;
using Zenject;

/// <summary>
/// Добавляет стартовых рабочих из дерева развития при запуске gameplay-сцены.
/// </summary>
public sealed class TechTreeStartingWorkersApplier : ValidatedMonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private House[] _targetHouses;

    private TechTreeBonusService _bonusService;

    private bool _applied;

    [Inject]
    private void Construct(TechTreeBonusService bonusService)
    {
        _bonusService = bonusService;
    }

    protected override bool ValidateInternal()
    {
        return ValidationUtility.NotEmptyArray(this, _targetHouses, nameof(_targetHouses));
    }

    private void Start()
    {
        ApplyBonus();
    }

    private void ApplyBonus()
    {
        if (_applied)
            return;

        _applied = true;

        int bonusWorkers = _bonusService.GetBonusInt(TechTreeBonusType.StartWorkers);

        if (bonusWorkers <= 0)
            return;

        for (int i = 0; i < _targetHouses.Length; i++)
        {
            House house = _targetHouses[i];

            if (house == null)
                continue;

            house.AddFreeWorkers(bonusWorkers);
        }

        Debug.Log($"[TechTreeStartingWorkersApplier] Добавлены стартовые рабочие: +{bonusWorkers}.");
    }
}