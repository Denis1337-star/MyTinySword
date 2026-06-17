using System;
using UnityEngine;

/// <summary>
/// Данные одного шага обучения.
/// </summary>
[Serializable]
public sealed class TutorialStepData
{
    [Header("Step")]
    [SerializeField] private TutorialStepType _stepType = TutorialStepType.Message;

    [TextArea]
    [SerializeField] private string _message;

    [SerializeField] private bool _allowManualNext = true;

    [Header("Build Step")]
    [SerializeField] private BuildingConfig _requiredBuildingConfig;

    [Header("Ensure Resources Before Step")]
    [SerializeField, Min(0)] private int _minimumWood;
    [SerializeField, Min(0)] private int _minimumGold;
    [SerializeField, Min(0)] private int _minimumMeat;

    public TutorialStepType StepType => _stepType;
    public string Message => _message;
    public bool AllowManualNext => _allowManualNext;

    public BuildingConfig RequiredBuildingConfig => _requiredBuildingConfig;

    public int MinimumWood => _minimumWood;
    public int MinimumGold => _minimumGold;
    public int MinimumMeat => _minimumMeat;

    public bool IsRequiredBuilding(BuildingConfig buildingConfig)
    {
        if (_requiredBuildingConfig == null)
            return buildingConfig != null;

        if (buildingConfig == null)
            return false;

        return buildingConfig == _requiredBuildingConfig ||
               buildingConfig.BuildingId == _requiredBuildingConfig.BuildingId;
    }
}