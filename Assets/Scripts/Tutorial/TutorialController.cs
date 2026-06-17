using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет пошаговым обучением.
/// Часть шагов можно листать кнопкой "Далее",
/// часть шагов ждёт реальные действия игрока.
/// </summary>
public sealed class TutorialController : ValidatedMonoBehaviour
{
    [Header("Level")]
    [SerializeField] private bool _runOnlyOnTutorialLevel = true;

    [Header("UI")]
    [SerializeField] private TutorialPanel _panel;

    [Header("Gameplay References")]
    [SerializeField] private HousePanel _housePanel;
    [SerializeField] private GameResultController _gameResultController;

    [Header("Steps")]
    [SerializeField] private List<TutorialStepData> _steps = new();

    private TutorialSaveService _tutorialSaveService;
    private LevelRuntimeService _levelRuntimeService;
    private SelectionSystem _selectionSystem;
    private BuildingRegistry _buildingRegistry;
    private ArmyUnitRegistry _armyUnitRegistry;
    private ResourceStorage _resourceStorage;
    private CommandSystem _commandSystem;

    private int _currentStepIndex;
    private bool _isRunning;
    private bool _subscribedToGameplayEvents;

    [Inject]
    private void Construct(
        TutorialSaveService tutorialSaveService,
        LevelRuntimeService levelRuntimeService,
        SelectionSystem selectionSystem,
        BuildingRegistry buildingRegistry,
        ArmyUnitRegistry armyUnitRegistry,
        ResourceStorage resourceStorage,
        CommandSystem commandSystem)
    {
        _tutorialSaveService = tutorialSaveService;
        _levelRuntimeService = levelRuntimeService;
        _selectionSystem = selectionSystem;
        _buildingRegistry = buildingRegistry;
        _armyUnitRegistry = armyUnitRegistry;
        _resourceStorage = resourceStorage;
        _commandSystem = commandSystem;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        _panel.Hide();
    }

    private void OnEnable()
    {
        _panel.NextButton.onClick.AddListener(GoToNextStep);
        _panel.SkipButton.onClick.AddListener(SkipTutorial);

        SubscribeToGameplayEvents();
    }

    private void Start()
    {
        TryStartTutorial();
    }

    private void OnDisable()
    {
        _panel.NextButton.onClick.RemoveListener(GoToNextStep);
        _panel.SkipButton.onClick.RemoveListener(SkipTutorial);

        UnsubscribeFromGameplayEvents();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _panel, nameof(_panel));
        valid &= ValidationUtility.IsAssigned(this, _housePanel, nameof(_housePanel));
        valid &= ValidationUtility.IsAssigned(this, _gameResultController, nameof(_gameResultController));

        if (_steps == null || _steps.Count == 0)
        {
            Debug.LogError($"{name}: список шагов обучения пуст.", this);
            valid = false;
        }

        return valid;
    }

    private void TryStartTutorial()
    {
        if (_tutorialSaveService.IsTutorialCompleted())
        {
            _panel.Hide();
            return;
        }

        if (_runOnlyOnTutorialLevel && !IsCurrentLevelTutorial())
        {
            _panel.Hide();
            return;
        }

        StartTutorial();
    }

    private bool IsCurrentLevelTutorial()
    {
        if (_levelRuntimeService == null || !_levelRuntimeService.HasCurrentLevel)
            return true;

        return _levelRuntimeService.CurrentLevel.IsTutorialLevel;
    }

    private void StartTutorial()
    {
        if (_steps == null || _steps.Count == 0)
            return;

        _isRunning = true;
        _currentStepIndex = 0;

        ShowCurrentStep();
    }

    private void GoToNextStep()
    {
        if (!_isRunning)
            return;

        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        if (!currentStep.AllowManualNext)
            return;

        AdvanceStep();
    }

    private void SkipTutorial()
    {
        if (!_isRunning)
            return;

        CompleteTutorial();
    }

    private void CompleteTutorial()
    {
        _isRunning = false;

        _tutorialSaveService.MarkTutorialCompleted();
        _panel.Hide();
    }

    private void AdvanceStep()
    {
        _currentStepIndex++;

        if (_currentStepIndex >= _steps.Count)
        {
            CompleteTutorial();
            return;
        }

        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
        {
            CompleteTutorial();
            return;
        }

        EnsureMinimumResources(currentStep);

        _panel.ShowStep(
            currentStep.Message,
            _currentStepIndex,
            _steps.Count);

        _panel.NextButton.gameObject.SetActive(currentStep.AllowManualNext);

        TryAutoCompleteCurrentStepFromCurrentState();
    }

    private TutorialStepData GetCurrentStep()
    {
        if (_steps == null)
            return null;

        if (_currentStepIndex < 0 || _currentStepIndex >= _steps.Count)
            return null;

        return _steps[_currentStepIndex];
    }

    private void EnsureMinimumResources(TutorialStepData step)
    {
        if (_resourceStorage == null || step == null)
            return;

        EnsureResource(ResourceType.Wood, step.MinimumWood);
        EnsureResource(ResourceType.Gold, step.MinimumGold);
        EnsureResource(ResourceType.Meat, step.MinimumMeat);
    }

    private void EnsureResource(ResourceType resourceType, int minimumAmount)
    {
        if (minimumAmount <= 0)
            return;

        int currentAmount = _resourceStorage.GetAmount(resourceType);

        if (currentAmount >= minimumAmount)
            return;

        int amountToAdd = minimumAmount - currentAmount;

        _resourceStorage.AddResource(resourceType, amountToAdd);

        Debug.Log(
            $"[TutorialController] Добавлено {amountToAdd} ресурса {resourceType}, чтобы tutorial не застрял.",
            this);
    }

    private void SubscribeToGameplayEvents()
    {
        if (_subscribedToGameplayEvents)
            return;

        if (_selectionSystem != null)
            _selectionSystem.SelectionChanged += OnSelectionChanged;

        if (_housePanel != null)
            _housePanel.AllWorkersJobAssigned += OnAllWorkersJobAssigned;

        if (_buildingRegistry != null)
            _buildingRegistry.BuildingBuilt += OnBuildingBuilt;

        if (_armyUnitRegistry != null)
            _armyUnitRegistry.OnArmyChanged += OnArmyChanged;

        if (_commandSystem != null)
            _commandSystem.AttackCommandIssued += OnAttackCommandIssued;

        if (_gameResultController != null)
            _gameResultController.GameFinished += OnGameFinished;

        _subscribedToGameplayEvents = true;
    }

    private void UnsubscribeFromGameplayEvents()
    {
        if (!_subscribedToGameplayEvents)
            return;

        if (_selectionSystem != null)
            _selectionSystem.SelectionChanged -= OnSelectionChanged;

        if (_housePanel != null)
            _housePanel.AllWorkersJobAssigned -= OnAllWorkersJobAssigned;

        if (_buildingRegistry != null)
            _buildingRegistry.BuildingBuilt -= OnBuildingBuilt;

        if (_armyUnitRegistry != null)
            _armyUnitRegistry.OnArmyChanged -= OnArmyChanged;

        if (_commandSystem != null)
            _commandSystem.AttackCommandIssued -= OnAttackCommandIssued;

        if (_gameResultController != null)
            _gameResultController.GameFinished -= OnGameFinished;

        _subscribedToGameplayEvents = false;
    }

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        if (!_isRunning)
            return;

        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        switch (currentStep.StepType)
        {
            case TutorialStepType.SelectHouse:
                TryCompleteSelectHouse(selectable);
                break;

            case TutorialStepType.SelectArmy:
                TryCompleteSelectArmy();
                break;
        }
    }

    private void TryCompleteSelectHouse(UnitSelectable selectable)
    {
        House house = FindComponentNearSelectable<House>(selectable);

        if (house == null)
            return;

        AdvanceStep();
    }

    private void TryCompleteSelectArmy()
    {
        if (!HasSelectedPlayerArmyUnit())
            return;

        AdvanceStep();
    }

    private void OnAllWorkersJobAssigned(WorkerJobType job)
    {
        if (!_isRunning)
            return;

        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        if (currentStep.StepType != TutorialStepType.AssignWorkersToWood)
            return;

        if (job != WorkerJobType.ChopWood)
            return;

        AdvanceStep();
    }

    private void OnBuildingBuilt(BuildingConfig buildingConfig)
    {
        if (!_isRunning)
            return;

        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        if (currentStep.StepType != TutorialStepType.BuildRequiredBuilding)
            return;

        if (!currentStep.IsRequiredBuilding(buildingConfig))
            return;

        AdvanceStep();
    }

    private void OnArmyChanged()
    {
        if (!_isRunning)
            return;

        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        if (currentStep.StepType != TutorialStepType.HireArmyUnit)
            return;

        if (_armyUnitRegistry == null)
            return;

        if (_armyUnitRegistry.CurrentPlayerArmyUnits <= 0)
            return;

        AdvanceStep();
    }

    private void OnAttackCommandIssued(IDamageable target)
    {
        if (!_isRunning)
            return;

        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        if (currentStep.StepType != TutorialStepType.AttackEnemy)
            return;

        if (target == null || target.IsDead)
            return;

        AdvanceStep();
    }

    private void OnGameFinished(bool victory)
    {
        if (!_isRunning)
            return;

        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        if (currentStep.StepType != TutorialStepType.WinLevel)
            return;

        if (!victory)
            return;

        CompleteTutorial();
    }

    private void TryAutoCompleteCurrentStepFromCurrentState()
    {
        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
            return;

        switch (currentStep.StepType)
        {
            case TutorialStepType.SelectHouse:
                TryAutoCompleteSelectHouse();
                break;

            case TutorialStepType.HireArmyUnit:
                TryAutoCompleteHireArmyUnit();
                break;

            case TutorialStepType.SelectArmy:
                TryAutoCompleteSelectArmy();
                break;
        }
    }

    private void TryAutoCompleteSelectHouse()
    {
        if (_selectionSystem == null)
            return;

        UnitSelectable currentSelection = _selectionSystem.CurrentSelection;

        if (currentSelection == null)
            return;

        House house = FindComponentNearSelectable<House>(currentSelection);

        if (house == null)
            return;

        AdvanceStep();
    }

    private void TryAutoCompleteHireArmyUnit()
    {
        if (_armyUnitRegistry == null)
            return;

        if (_armyUnitRegistry.CurrentPlayerArmyUnits <= 0)
            return;

        AdvanceStep();
    }

    private void TryAutoCompleteSelectArmy()
    {
        if (!HasSelectedPlayerArmyUnit())
            return;

        AdvanceStep();
    }

    private bool HasSelectedPlayerArmyUnit()
    {
        if (_selectionSystem == null)
            return false;

        IReadOnlyList<UnitSelectable> selectedUnits = _selectionSystem.SelectedUnits;

        if (selectedUnits == null || selectedUnits.Count == 0)
            return false;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            UnitSelectable selectable = selectedUnits[i];

            if (selectable == null)
                continue;

            ArmyUnit armyUnit = FindComponentNearSelectable<ArmyUnit>(selectable);

            if (armyUnit == null)
                continue;

            if (!armyUnit.IsPlayerUnit())
                continue;

            if (armyUnit.IsDead)
                continue;

            return true;
        }

        return false;
    }

    private T FindComponentNearSelectable<T>(UnitSelectable selectable) where T : Component
    {
        if (selectable == null)
            return null;

        T component = selectable.GetComponent<T>();

        if (component != null)
            return component;

        component = selectable.GetComponentInParent<T>();

        if (component != null)
            return component;

        return selectable.GetComponentInChildren<T>();
    }
}