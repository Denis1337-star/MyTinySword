using UnityEngine;
using Zenject;

/// <summary>
/// Главный installer сцены.
/// </summary>
public sealed class GameSceneInstaller : MonoInstaller
{
    [Header("Core Scene Services")]
    [SerializeField] private ResourceStorage _resourceStorage;

    [Header("Tech Tree Bootstrap")]
    [SerializeField] private House _playerHouse;

    [Header("Army")]
    [SerializeField, Min(1)] private int _maxPlayerArmyUnits = 10;

    [Header("Selection / Camera")]
    [SerializeField] private SelectionSystem _selectionSystem;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraFocusController _cameraFocusController;
    [SerializeField] private BuildingDemolishConfirmPanel _buildingDemolishConfirmPanel;

    [Header("Input / Commands")]
    [SerializeField] private GameplayInputController _gameplayInputController;
    [SerializeField] private CommandSystem _commandSystem;

    [Header("UI")]
    [SerializeField] private Canvas _screenCanvas;
    [SerializeField] private UiSoundRouter _uiSoundRouter;
    [SerializeField] private WorkerListPanel _workerListPanel;
    [SerializeField] private SelectionUiPresenter _selectionUiPresenter;
    [SerializeField] private GameResultController _gameResultController;

    public override void InstallBindings()
    {
        if (!ValidateReferences())
            return;

        BindCoreServices();
        BindRegistries();
        BindSelectionAndCamera();
        BindInputAndCommands();
        BindUi();
        BindFactories();
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _resourceStorage, nameof(_resourceStorage));

        valid &= ValidationUtility.IsAssigned(this, _selectionSystem, nameof(_selectionSystem));
        valid &= ValidationUtility.IsAssigned(this, _mainCamera, nameof(_mainCamera));
        valid &= ValidationUtility.IsAssigned(this, _cameraFocusController, nameof(_cameraFocusController));
        valid &= ValidationUtility.IsAssigned(this, _buildingDemolishConfirmPanel, nameof(_buildingDemolishConfirmPanel));
        valid &= ValidationUtility.IsAssigned(this, _gameResultController, nameof(_gameResultController));
        valid &= ValidationUtility.IsAssigned(this, _gameplayInputController, nameof(_gameplayInputController));
        valid &= ValidationUtility.IsAssigned(this, _commandSystem, nameof(_commandSystem));

        valid &= ValidationUtility.IsAssigned(this, _screenCanvas, nameof(_screenCanvas));
        valid &= ValidationUtility.IsAssigned(this, _uiSoundRouter, nameof(_uiSoundRouter));
        valid &= ValidationUtility.IsAssigned(this, _workerListPanel, nameof(_workerListPanel));
        valid &= ValidationUtility.IsAssigned(this, _selectionUiPresenter, nameof(_selectionUiPresenter));
        valid &= ValidationUtility.IsAssigned(this, _playerHouse, nameof(_playerHouse));

        return valid;
    }

    private void BindCoreServices()
    {
        Container.Bind<ResourceStorage>()
            .FromInstance(_resourceStorage)
            .AsSingle()
            .NonLazy();

        Container.BindInterfacesTo<GameplayTechTreeBootstrap>()
    .AsSingle()
    .WithArguments(_playerHouse)
    .NonLazy();
    }

    private void BindRegistries()
    {
        Container.Bind<WorkerRegistry>()
            .AsSingle()
            .NonLazy();

        Container.Bind<ResourceRegistry>()
            .AsSingle()
            .NonLazy();

        Container.Bind<BuildingRegistry>()
            .AsSingle()
            .NonLazy();

        Container.Bind<ArmyUnitRegistry>()
            .AsSingle()
            .WithArguments(_maxPlayerArmyUnits)
            .NonLazy();
    }

    private void BindSelectionAndCamera()
    {
        Container.Bind<SelectionSystem>()
            .FromInstance(_selectionSystem)
            .AsSingle()
            .NonLazy();

        Container.Bind<Camera>()
            .FromInstance(_mainCamera)
            .AsSingle()
            .NonLazy();

        Container.Bind<EnemyHealthInspectService>()
    .AsSingle()
    .NonLazy();

        Container.Bind<CameraFocusController>()
            .FromInstance(_cameraFocusController)
            .AsSingle()
            .NonLazy();

        Container.Bind<BuildingDemolishService>()
            .AsSingle()
            .NonLazy();
    }

    private void BindInputAndCommands()
    {
        Container.Bind<GameplayInputController>()
            .FromInstance(_gameplayInputController)
            .AsSingle()
            .NonLazy();

        Container.Bind<CommandSystem>()
            .FromInstance(_commandSystem)
            .AsSingle()
            .NonLazy();
    }

    private void BindUi()
    {
        Container.Bind<Canvas>()
            .FromInstance(_screenCanvas)
            .AsSingle()
            .NonLazy();

        Container.Bind<UiSoundRouter>()
            .FromInstance(_uiSoundRouter)
            .AsSingle()
            .NonLazy();

        Container.Bind<SelectionUiPresenter>()
            .FromInstance(_selectionUiPresenter)
            .AsSingle()
            .NonLazy();

        Container.Bind<WorkerListPanel>()
            .FromInstance(_workerListPanel)
            .AsSingle()
            .NonLazy();

        Container.Bind<BuildingDemolishConfirmPanel>()
            .FromInstance(_buildingDemolishConfirmPanel)
            .AsSingle();

        Container.Bind<GameResultController>()
             .FromInstance(_gameResultController)
             .AsSingle();
    }

    private void BindFactories()
    {
        Container.Bind<WorkerFactory>()
            .AsSingle()
            .NonLazy();

        Container.Bind<BuildingFactory>()
            .AsSingle()
            .NonLazy();

        Container.Bind<ArmyUnitFactory>()
            .AsSingle()
            .NonLazy();
    }
}