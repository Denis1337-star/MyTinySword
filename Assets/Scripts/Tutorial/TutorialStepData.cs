using System;
using UnityEngine;

/// <summary>
/// Настройки одного шага обучения в инспекторе.
/// </summary>
[Serializable]
public sealed class TutorialStepData
{
    [Header("Step")]
    [SerializeField] private TutorialStepType _stepType = TutorialStepType.Message;

    [TextArea]
    [SerializeField] private string _message;
    [TextArea]
    [SerializeField] private string _messageEn;

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

    /// <summary>
    /// Проверяет, подходит ли конфиг здания для шага (например, казарма).
    /// </summary>
    public bool IsRequiredBuilding(BuildingConfig buildingConfig)
    {
        return BuildingConfigUtility.Matches(_requiredBuildingConfig, buildingConfig);
    }
    public string GetMessage(string lang)
    {
        if (lang == "en" && !string.IsNullOrWhiteSpace(_messageEn))
            return _messageEn;
        return _message;
    }
}
