/// <summary>
/// Режим отображения UI на шаге обучения.
/// </summary>
public enum TutorialUiMode
{
    FullScreenBlock = 0,
    FullScreenInfo = 1,
    GuidedBanner = 2,
    GuidedUiButton = 3,
    GuidedUiPanel = 4
}

/// <summary>
/// UI и gameplay-правила одного шага tutorial.
/// </summary>
public readonly struct TutorialStepDefinition
{
    public TutorialUiMode UiMode { get; }
    public bool DimBlocksInput { get; }
    public bool DimVisible { get; }
    public TutorialHighlightTarget Highlight { get; }

    public bool AllowsWorldGameplayInput { get; }
    public bool AllowsSelectionWithoutRoot { get; }
    public bool AllowsClearSelection { get; }
    public bool AllowsArmyMoveCommand { get; }
    public bool AllowsEnemyInspect { get; }
    public bool AllowsAttackCommand { get; }
    public bool AllowsDemolishBuilding { get; }

    public TutorialStepDefinition(
        TutorialUiMode uiMode,
        bool dimBlocksInput,
        TutorialHighlightTarget highlight,
        bool dimVisible,
        bool allowsWorldGameplayInput,
        bool allowsSelectionWithoutRoot,
        bool allowsClearSelection,
        bool allowsArmyMoveCommand,
        bool allowsEnemyInspect,
        bool allowsAttackCommand,
        bool allowsDemolishBuilding)
    {
        UiMode = uiMode;
        DimBlocksInput = dimBlocksInput;
        Highlight = highlight;
        DimVisible = dimVisible;
        AllowsWorldGameplayInput = allowsWorldGameplayInput;
        AllowsSelectionWithoutRoot = allowsSelectionWithoutRoot;
        AllowsClearSelection = allowsClearSelection;
        AllowsArmyMoveCommand = allowsArmyMoveCommand;
        AllowsEnemyInspect = allowsEnemyInspect;
        AllowsAttackCommand = allowsAttackCommand;
        AllowsDemolishBuilding = allowsDemolishBuilding;
    }

    public static TutorialStepDefinition For(TutorialStepType stepType)
    {
        return stepType switch
        {
            TutorialStepType.Message => BlockedFullScreen(
                allowsSelectionWithoutRoot: false,
                allowsClearSelection: false),

            TutorialStepType.SelectHouse => WorldTarget(
                TutorialHighlightTarget.HouseOnMap,
                allowsWorldGameplayInput: true,
                allowsClearSelection: false),

            TutorialStepType.AssignWorkersToWood => UiButton(
                TutorialHighlightTarget.AssignAllWoodButton,
                allowsSelectionWithoutRoot: false,
                allowsClearSelection: false),

            TutorialStepType.SelectConstructionSlot => WorldTarget(
                TutorialHighlightTarget.ConstructionSlot,
                allowsWorldGameplayInput: true,
                allowsClearSelection: false),

            TutorialStepType.BuildBarrackInPanel => UiPanel(
                TutorialHighlightTarget.ConstructionPanel,
                allowsSelectionWithoutRoot: false,
                allowsClearSelection: false),

            TutorialStepType.WaitBuildingConstructed => WaitBannerOnly(),

            TutorialStepType.SelectBuiltBarrack => WorldTarget(
                TutorialHighlightTarget.BuiltBarrack,
                allowsWorldGameplayInput: true,
                allowsClearSelection: false),

            TutorialStepType.HireArmyUnit => UiPanel(
                TutorialHighlightTarget.ProductionBuildingPanel,
                allowsSelectionWithoutRoot: false,
                allowsClearSelection: false,
                allowsDemolishBuilding: false),

            TutorialStepType.WaitWarriorSpawn => WaitBannerOnly(),

            TutorialStepType.SelectArmy => WorldTarget(
                TutorialHighlightTarget.PlayerWarrior,
                allowsWorldGameplayInput: true,
                allowsClearSelection: false,
                allowsArmyMoveCommand: true),

            TutorialStepType.FocusEnemy => WorldTarget(
                TutorialHighlightTarget.EnemyWarrior,
                allowsWorldGameplayInput: true,
                allowsClearSelection: false,
                allowsArmyMoveCommand: true,
                allowsEnemyInspect: true),

            TutorialStepType.AttackEnemy => WorldTarget(
                TutorialHighlightTarget.EnemyWarrior,
                allowsWorldGameplayInput: true,
                allowsClearSelection: false,
                allowsArmyMoveCommand: true,
                allowsEnemyInspect: true,
                allowsAttackCommand: true),

            TutorialStepType.WaitBattleReach => BannerOnly(
                allowsArmyMoveCommand: true,
                allowsEnemyInspect: true,
                allowsAttackCommand: true),

            TutorialStepType.FinalMotivation => BlockedFullScreen(
                allowsSelectionWithoutRoot: false,
                allowsClearSelection: false),

            TutorialStepType.WinLevel => InfoFullScreen(
                allowsWorldGameplayInput: true,
                allowsArmyMoveCommand: true,
                allowsEnemyInspect: true,
                allowsAttackCommand: true),

            _ => BlockedFullScreen(
                allowsSelectionWithoutRoot: false,
                allowsClearSelection: false)
        };
    }

    private static TutorialStepDefinition BlockedFullScreen(
        bool allowsSelectionWithoutRoot,
        bool allowsClearSelection)
    {
        return new TutorialStepDefinition(
            TutorialUiMode.FullScreenBlock,
            true,
            TutorialHighlightTarget.None,
            true,
            false,
            allowsSelectionWithoutRoot,
            allowsClearSelection,
            false,
            false,
            false,
            true);
    }

    private static TutorialStepDefinition InfoFullScreen(
        bool allowsWorldGameplayInput,
        bool allowsArmyMoveCommand,
        bool allowsEnemyInspect,
        bool allowsAttackCommand)
    {
        return new TutorialStepDefinition(
            TutorialUiMode.FullScreenInfo,
            false,
            TutorialHighlightTarget.None,
            true,
            allowsWorldGameplayInput,
            true,
            true,
            allowsArmyMoveCommand,
            allowsEnemyInspect,
            allowsAttackCommand,
            true);
    }

    private static TutorialStepDefinition WaitBannerOnly()
    {
        return new TutorialStepDefinition(
            TutorialUiMode.GuidedBanner,
            false,
            TutorialHighlightTarget.None,
            false,
            false,
            false,
            true,
            false,
            false,
            false,
            true);
    }

    private static TutorialStepDefinition BannerOnly(
        bool allowsArmyMoveCommand = false,
        bool allowsEnemyInspect = false,
        bool allowsAttackCommand = false)
    {
        return new TutorialStepDefinition(
            TutorialUiMode.GuidedBanner,
            false,
            TutorialHighlightTarget.None,
            false,
            false,
            true,
            true,
            allowsArmyMoveCommand,
            allowsEnemyInspect,
            allowsAttackCommand,
            true);
    }

    private static TutorialStepDefinition WorldTarget(
        TutorialHighlightTarget highlight,
        bool allowsWorldGameplayInput = false,
        bool allowsClearSelection = true,
        bool allowsArmyMoveCommand = false,
        bool allowsEnemyInspect = false,
        bool allowsAttackCommand = false,
        bool allowsDemolishBuilding = true)
    {
        return new TutorialStepDefinition(
            TutorialUiMode.GuidedBanner,
            false,
            highlight,
            true,
            allowsWorldGameplayInput,
            true,
            allowsClearSelection,
            allowsArmyMoveCommand,
            allowsEnemyInspect,
            allowsAttackCommand,
            allowsDemolishBuilding);
    }

    private static TutorialStepDefinition UiButton(
        TutorialHighlightTarget highlight,
        bool allowsSelectionWithoutRoot,
        bool allowsClearSelection)
    {
        return new TutorialStepDefinition(
            TutorialUiMode.GuidedUiButton,
            true,
            highlight,
            true,
            false,
            allowsSelectionWithoutRoot,
            allowsClearSelection,
            false,
            false,
            false,
            true);
    }

    private static TutorialStepDefinition UiPanel(
        TutorialHighlightTarget highlight,
        bool allowsSelectionWithoutRoot,
        bool allowsClearSelection,
        bool allowsDemolishBuilding = true)
    {
        return new TutorialStepDefinition(
            TutorialUiMode.GuidedUiPanel,
            true,
            highlight,
            true,
            false,
            allowsSelectionWithoutRoot,
            allowsClearSelection,
            false,
            false,
            false,
            allowsDemolishBuilding);
    }
}
