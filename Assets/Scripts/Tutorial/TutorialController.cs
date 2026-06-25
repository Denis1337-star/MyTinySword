using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет прохождением обучения: шаги, ограничения ввода, камера и сохранение после победы.
/// </summary>
public sealed class TutorialController : ValidatedMonoBehaviour
{
    [Header("Level")]
    [SerializeField] private bool _runOnlyOnTutorialLevel = true;

    [Header("UI")]
    [SerializeField] private TutorialUiView _uiView;
    [SerializeField] private CameraFocusController _cameraFocus;

    [Header("Highlight Targets")]
    [SerializeField] private Transform _houseHighlightTarget;
    [SerializeField] private Transform _constructionSlotHighlightTarget;
    [SerializeField] private Transform _playerWarriorHighlightTarget;
    [SerializeField] private Transform _enemyWarriorHighlightTarget;

    [Header("Gameplay References")]
    [SerializeField] private HousePanel _housePanel;
    [SerializeField] private ConstructionPanel _constructionPanel;
    [SerializeField] private ProductionBuildingPanel _productionBuildingPanel;
    [SerializeField] private GameResultController _gameResultController;

    [Header("Steps")]
    [SerializeField] private List<TutorialStepData> _steps = new();

    private readonly List<ArmyUnit> _armyUnitBuffer = new();

    private TutorialSaveService _tutorialSaveService;
    private LevelRuntimeService _levelRuntimeService;
    private SelectionSystem _selectionSystem;
    private SelectionUiPresenter _selectionUiPresenter;
    private BuildingRegistry _buildingRegistry;
    private ArmyUnitRegistry _armyUnitRegistry;
    private ResourceStorage _resourceStorage;
    private CommandSystem _commandSystem;
    private Camera _mainCamera;

    private Transform _builtBarrackTarget;
    private ArmyUnit _trackedWarrior;
    private int _playerArmyCountBeforeSpawn;
    private int _currentStepIndex;
    private bool _isRunning;
    private bool _awaitVictoryToSave;
    private bool _subscribedToGameplayEvents;

    [Inject]
    private void Construct(
        TutorialSaveService tutorialSaveService,
        LevelRuntimeService levelRuntimeService,
        SelectionSystem selectionSystem,
        SelectionUiPresenter selectionUiPresenter,
        BuildingRegistry buildingRegistry,
        ArmyUnitRegistry armyUnitRegistry,
        ResourceStorage resourceStorage,
        CommandSystem commandSystem,
        Camera mainCamera)
    {
        _tutorialSaveService = tutorialSaveService;
        _levelRuntimeService = levelRuntimeService;
        _selectionSystem = selectionSystem;
        _selectionUiPresenter = selectionUiPresenter;
        _buildingRegistry = buildingRegistry;
        _armyUnitRegistry = armyUnitRegistry;
        _resourceStorage = resourceStorage;
        _commandSystem = commandSystem;
        _mainCamera = mainCamera;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        _uiView.HideAll();
    }

    private void OnEnable()
    {
        _uiView.FullScreenNextButton.onClick.AddListener(GoToNextStep);

        SubscribeToGameplayEvents();
    }

    private void OnDisable()
    {
        _uiView.FullScreenNextButton.onClick.RemoveListener(GoToNextStep);

        UnsubscribeFromGameplayEvents();
        _cameraFocus.StopTutorialCamera();
        ClearConstructionTutorialRestrictions();
    }

    private void Update()
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType == TutorialStepType.WaitBattleReach)
            UpdateWaitBattleReachStep();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _uiView, nameof(_uiView));
        valid &= ValidationUtility.IsAssigned(this, _cameraFocus, nameof(_cameraFocus));
        valid &= ValidationUtility.IsAssigned(this, _housePanel, nameof(_housePanel));
        valid &= ValidationUtility.IsAssigned(this, _constructionPanel, nameof(_constructionPanel));
        valid &= ValidationUtility.IsAssigned(this, _productionBuildingPanel, nameof(_productionBuildingPanel));
        valid &= ValidationUtility.IsAssigned(this, _gameResultController, nameof(_gameResultController));
        valid &= ValidationUtility.NotEmptyList(this, _steps, nameof(_steps));

        return valid;
    }

    private void Start()
    {
        TryStartTutorial();
    }

    private void TryStartTutorial()
    {
        if (_tutorialSaveService.IsTutorialCompleted())
        {
            _uiView.HideAll();
            return;
        }

        if (_runOnlyOnTutorialLevel && !IsCurrentLevelTutorial())
        {
            _uiView.HideAll();
            return;
        }

        StartTutorial();
    }

    private bool IsCurrentLevelTutorial()
    {
        if (!_levelRuntimeService.HasCurrentLevel)
            return true;

        return _levelRuntimeService.CurrentLevel.IsTutorialLevel;
    }

    private void StartTutorial()
    {
        _isRunning = true;
        _awaitVictoryToSave = false;
        _currentStepIndex = 0;
        _cameraFocus.BeginTutorialCamera();
        ShowCurrentStep();
    }

    private void GoToNextStep()
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (!currentStep.AllowManualNext)
            return;

        if (currentStep.StepType == TutorialStepType.FinalMotivation)
        {
            FinishGuidanceAndAwaitVictory();
            return;
        }

        AdvanceStep();
    }

    /// <summary>
    /// Завершает обучение и сразу помечает его пройденным.
    /// </summary>
    private void CompleteTutorial()
    {
        _isRunning = false;
        _awaitVictoryToSave = false;
        _tutorialSaveService.MarkTutorialCompleted();
        ClearTutorialRestrictions();
        _cameraFocus.EndTutorialCamera();
        _uiView.HideAll();
    }

    /// <summary>
    /// Снимает ограничения UI, но сохранение произойдёт только после победы.
    /// </summary>
    private void FinishGuidanceAndAwaitVictory()
    {
        _isRunning = false;
        _awaitVictoryToSave = true;
        ClearTutorialRestrictions();
        _cameraFocus.EndTutorialCamera();
        _uiView.HideAll();
    }

    private void AdvanceStep()
    {
        ExitCurrentStep();

        _currentStepIndex++;

        if (_currentStepIndex >= _steps.Count)
        {
            FinishGuidanceAndAwaitVictory();
            return;
        }

        ShowCurrentStep();
    }

    private void ExitCurrentStep()
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType == TutorialStepType.AssignWorkersToWood)
        {
            _selectionUiPresenter.HideHousePanel();
            _selectionSystem.ForceClearSelection();
        }

        if (currentStep.StepType == TutorialStepType.BuildBarrackInPanel)
            ClearConstructionTutorialRestrictions();
    }

    private void ShowCurrentStep()
    {
        TutorialStepData currentStep = GetCurrentStep();

        if (currentStep == null)
        {
            FinishGuidanceAndAwaitVictory();
            return;
        }

        EnsureMinimumResources(currentStep);
        ApplyStepPreparation(currentStep);

        TutorialStepDefinition definition = TutorialStepDefinition.For(currentStep.StepType);
        Transform worldTarget = ResolveWorldTarget(definition.Highlight, currentStep);
        RectTransform uiTarget = ResolveUiTarget(definition.Highlight, currentStep);

        _uiView.Present(
            currentStep,
            _currentStepIndex,
            _steps.Count,
            definition,
            _mainCamera,
            uiTarget,
            worldTarget);

        TutorialInputGuard.Apply(
            currentStep.StepType,
            ResolveAllowedSelectionRoot(worldTarget));

        TryAutoCompleteCurrentStepFromCurrentState();
        TryStartStepPresentation(currentStep);
    }

    private void ApplyStepPreparation(TutorialStepData step)
    {
        switch (step.StepType)
        {
            case TutorialStepType.BuildBarrackInPanel:
                _constructionPanel.SetTutorialAllowedBuilding(step.RequiredBuildingConfig);
                break;

            case TutorialStepType.WaitWarriorSpawn:
                _playerArmyCountBeforeSpawn = _armyUnitRegistry.CurrentPlayerArmyUnits;
                break;
        }
    }

    private void TryStartStepPresentation(TutorialStepData step)
    {
        if (step.StepType is TutorialStepType.FocusEnemy or TutorialStepType.AttackEnemy)
        {
            Transform enemyTarget = ResolveEnemyWarriorTarget();

            if (enemyTarget != null)
                _cameraFocus.TutorialFocusOn(enemyTarget);
        }
    }

    private Component ResolveAllowedSelectionRoot(Transform worldTarget)
    {
        return worldTarget;
    }

    private RectTransform ResolveUiTarget(TutorialHighlightTarget target, TutorialStepData step)
    {
        return target switch
        {
            TutorialHighlightTarget.AssignAllWoodButton => _housePanel.AssignAllWoodButtonRect,
            TutorialHighlightTarget.ProductionHireButton => _productionBuildingPanel.HireButtonRect,
            TutorialHighlightTarget.ProductionBuildingPanel => _productionBuildingPanel.PanelRect,
            TutorialHighlightTarget.ConstructionPanel => _constructionPanel.PanelRect,
            _ => null
        };
    }

    private Transform ResolveWorldTarget(TutorialHighlightTarget target, TutorialStepData step)
    {
        return target switch
        {
            TutorialHighlightTarget.HouseOnMap => _houseHighlightTarget,
            TutorialHighlightTarget.ConstructionSlot => _constructionSlotHighlightTarget,
            TutorialHighlightTarget.BuiltBarrack => ResolveBuiltBarrackTarget(step),
            TutorialHighlightTarget.PlayerWarrior => ResolvePlayerWarriorTarget(),
            TutorialHighlightTarget.EnemyWarrior => ResolveEnemyWarriorTarget(),
            _ => null
        };
    }

    private Transform ResolveBuiltBarrackTarget(TutorialStepData step)
    {
        if (_builtBarrackTarget != null)
            return _builtBarrackTarget;

        return FindBuiltBuildingTransform(step.RequiredBuildingConfig);
    }

    private Transform ResolvePlayerWarriorTarget()
    {
        if (_playerWarriorHighlightTarget != null)
            return _playerWarriorHighlightTarget;

        ArmyUnit warrior = FindFirstPlayerWarrior();

        return warrior != null ? warrior.transform : null;
    }

    private Transform ResolveEnemyWarriorTarget()
    {
        if (_enemyWarriorHighlightTarget != null)
            return _enemyWarriorHighlightTarget;

        ArmyUnit warrior = FindFirstEnemyWarrior();

        return warrior != null ? warrior.transform : null;
    }

    private ArmyUnit FindFirstPlayerWarrior()
    {
        _armyUnitRegistry.GetAllPlayerUnitsNonAlloc(_armyUnitBuffer);

        for (int i = 0; i < _armyUnitBuffer.Count; i++)
        {
            ArmyUnit unit = _armyUnitBuffer[i];

            if (unit != null && !unit.IsDead)
                return unit;
        }

        return null;
    }

    private ArmyUnit FindFirstEnemyWarrior()
    {
        IReadOnlyList<ArmyUnit> allUnits = _armyUnitRegistry.AllUnits;

        for (int i = 0; i < allUnits.Count; i++)
        {
            ArmyUnit unit = allUnits[i];

            if (unit == null || unit.IsDead || unit.IsPlayerUnit())
                continue;

            return unit;
        }

        return null;
    }

    private Transform FindBuiltBuildingTransform(BuildingConfig buildingConfig)
    {
        return _buildingRegistry.FindBuiltBuildingTransform(buildingConfig);
    }

    private TutorialStepData GetCurrentStep()
    {
        if (_currentStepIndex < 0 || _currentStepIndex >= _steps.Count)
            return null;

        return _steps[_currentStepIndex];
    }

    private bool TryGetRunningStep(out TutorialStepData step)
    {
        step = GetCurrentStep();
        return _isRunning && step != null;
    }

    private void EnsureMinimumResources(TutorialStepData step)
    {
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

        _resourceStorage.AddResource(resourceType, minimumAmount - currentAmount);
    }

    private void ClearTutorialRestrictions()
    {
        TutorialInputGuard.Clear();
        ClearConstructionTutorialRestrictions();
    }

    private void ClearConstructionTutorialRestrictions()
    {
        _constructionPanel.SetTutorialAllowedBuilding(null);
    }

    private void SubscribeToGameplayEvents()
    {
        if (_subscribedToGameplayEvents)
            return;

        _selectionSystem.SelectionChanged += OnSelectionChanged;
        _housePanel.AllWorkersJobAssigned += OnAllWorkersJobAssigned;
        _constructionPanel.ConstructionStarted += OnConstructionStarted;
        _buildingRegistry.BuildingBuilt += OnBuildingBuilt;
        _productionBuildingPanel.UnitHired += OnUnitHired;
        _armyUnitRegistry.OnArmyChanged += OnArmyChanged;
        _commandSystem.AttackCommandIssued += OnAttackCommandIssued;
        _gameResultController.GameFinished += OnGameFinished;

        _subscribedToGameplayEvents = true;
    }

    private void UnsubscribeFromGameplayEvents()
    {
        if (!_subscribedToGameplayEvents)
            return;

        _selectionSystem.SelectionChanged -= OnSelectionChanged;
        _housePanel.AllWorkersJobAssigned -= OnAllWorkersJobAssigned;
        _constructionPanel.ConstructionStarted -= OnConstructionStarted;
        _buildingRegistry.BuildingBuilt -= OnBuildingBuilt;
        _productionBuildingPanel.UnitHired -= OnUnitHired;
        _armyUnitRegistry.OnArmyChanged -= OnArmyChanged;
        _commandSystem.AttackCommandIssued -= OnAttackCommandIssued;
        _gameResultController.GameFinished -= OnGameFinished;

        _subscribedToGameplayEvents = false;
    }

    private void OnSelectionChanged(UnitSelectable selectable)
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        switch (currentStep.StepType)
        {
            case TutorialStepType.SelectHouse:
                TryCompleteSelectHouse(selectable);
                break;

            case TutorialStepType.SelectConstructionSlot:
                TryCompleteSelectConstructionSlot(selectable);
                break;

            case TutorialStepType.SelectBuiltBarrack:
                TryCompleteSelectBuiltBarrack(selectable);
                break;

            case TutorialStepType.SelectArmy:
                TryCompleteSelectArmy();
                break;
        }
    }

    private void TryCompleteSelectHouse(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        if (SelectableUtility.FindNear<House>(selectable) == null)
            return;

        AdvanceStep();
    }

    private void TryCompleteSelectConstructionSlot(UnitSelectable selectable)
    {
        if (selectable == null)
            return;

        if (SelectableUtility.FindNear<ConstructionSlot>(selectable) == null)
            return;

        AdvanceStep();
    }

    private void TryCompleteSelectBuiltBarrack(UnitSelectable selectable)
    {
        if (selectable == null || !TryGetRunningStep(out TutorialStepData currentStep))
            return;

        ProductionBuildingBase building = SelectableUtility.FindNear<ProductionBuildingBase>(selectable);

        if (building == null || !currentStep.IsRequiredBuilding(building.Config))
            return;

        AdvanceStep();
    }

    private void TryCompleteSelectArmy()
    {
        if (!ArmyUnitSelectionUtility.HasAnyPlayerArmyUnit(_selectionSystem.SelectedUnits))
            return;

        AdvanceStep();
    }

    private void TryCompleteWaitBuildingConstructed(TutorialStepData step)
    {
        Transform built = FindBuiltBuildingTransform(step.RequiredBuildingConfig);

        if (built == null)
            return;

        _builtBarrackTarget = built;
        AdvanceStep();
    }

    private void OnAllWorkersJobAssigned(WorkerJobType job)
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType != TutorialStepType.AssignWorkersToWood)
            return;

        if (job != WorkerJobType.ChopWood)
            return;

        AdvanceStep();
    }

    private void OnConstructionStarted(BuildingConfig buildingConfig)
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType != TutorialStepType.BuildBarrackInPanel)
            return;

        if (!currentStep.IsRequiredBuilding(buildingConfig))
            return;

        AdvanceStep();
    }

    private void OnBuildingBuilt(BuildingConfig buildingConfig)
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType != TutorialStepType.WaitBuildingConstructed)
            return;

        if (!currentStep.IsRequiredBuilding(buildingConfig))
            return;

        _builtBarrackTarget = FindBuiltBuildingTransform(buildingConfig);
        AdvanceStep();
    }

    private void OnUnitHired()
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType != TutorialStepType.HireArmyUnit)
            return;

        _productionBuildingPanel.Dismiss();
        AdvanceStep();
    }

    private void OnArmyChanged()
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType == TutorialStepType.WaitWarriorSpawn)
        {
            if (_armyUnitRegistry.CurrentPlayerArmyUnits > _playerArmyCountBeforeSpawn)
                AdvanceStep();

            return;
        }

        if (currentStep.StepType == TutorialStepType.SelectArmy)
            RefreshCurrentStepPresentation();
    }

    private void OnAttackCommandIssued(IDamageable target)
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType != TutorialStepType.AttackEnemy)
            return;

        if (target == null || target.IsDead)
            return;

        _trackedWarrior = FindFirstPlayerWarrior();
        AdvanceStep();
    }

    private void OnGameFinished(bool victory)
    {
        if (_awaitVictoryToSave && victory)
        {
            CompleteTutorial();
            return;
        }

        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        if (currentStep.StepType != TutorialStepType.WinLevel)
            return;

        if (!victory)
            return;

        CompleteTutorial();
    }

    private void UpdateWaitBattleReachStep()
    {
        if (_trackedWarrior == null)
            _trackedWarrior = FindFirstPlayerWarrior();

        if (_trackedWarrior != null && !_cameraFocus.IsTutorialFollowActive)
        {
            _cameraFocus.TutorialFollow(
                _trackedWarrior.transform,
                () => _trackedWarrior == null || _trackedWarrior.IsDead);
        }

        if (_trackedWarrior != null && !_trackedWarrior.IsDead)
            return;

        _cameraFocus.StopTutorialCamera();
        AdvanceStep();
    }

    private void RefreshCurrentStepPresentation()
    {
        if (!_isRunning)
            return;

        ShowCurrentStep();
    }

    /// <summary>
    /// Если игрок уже выполнил действие до показа шага — переходим дальше без лишнего клика.
    /// </summary>
    private void TryAutoCompleteCurrentStepFromCurrentState()
    {
        if (!TryGetRunningStep(out TutorialStepData currentStep))
            return;

        switch (currentStep.StepType)
        {
            case TutorialStepType.SelectHouse:
                TryCompleteSelectHouse(_selectionSystem.CurrentSelection);
                break;

            case TutorialStepType.SelectConstructionSlot:
                TryCompleteSelectConstructionSlot(_selectionSystem.CurrentSelection);
                break;

            case TutorialStepType.SelectBuiltBarrack:
                TryCompleteSelectBuiltBarrack(_selectionSystem.CurrentSelection);
                break;

            case TutorialStepType.WaitWarriorSpawn:
                if (_armyUnitRegistry.CurrentPlayerArmyUnits > _playerArmyCountBeforeSpawn)
                    AdvanceStep();
                break;

            case TutorialStepType.SelectArmy:
                TryCompleteSelectArmy();
                break;

            case TutorialStepType.WaitBuildingConstructed:
                TryCompleteWaitBuildingConstructed(currentStep);
                break;
        }
    }

    private bool HasSelectedPlayerArmyUnit()
    {
        return ArmyUnitSelectionUtility.HasAnyPlayerArmyUnit(_selectionSystem.SelectedUnits);
    }
}
