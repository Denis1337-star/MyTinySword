using UnityEngine;
using Zenject;

/// <summary>
/// Главный installer сцены
/// </summary>
public class GameSceneInstaller : MonoInstaller
{
    [Header("Core Scene Services")]
    [SerializeField] private ResourceStorage _resourceStorage;

    [Header("Registries")]
    [SerializeField] private WorkerRegistry _workerRegistry;
    [SerializeField] private ResourceRegistry _resourceRegistry;
    [SerializeField] private BuildingRegistry _buildingRegistry;
    [SerializeField] private ArmyUnitRegistry _armyUnitRegistry;

    [Header("Selection / Camera")]
    [SerializeField] private SelectionSystem _selectionSystem;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraFocusController _cameraFocusController;

    [Header("Input / Commands")]
    [SerializeField] private GameplayInputController _gameplayInputController;
    [SerializeField] private CommandSystem _commandSystem;

    [Header("UI")]
    [SerializeField] private Canvas _screenCanvas;
    [SerializeField] private ConstructionPanel _constructionPanel;
    [SerializeField] private WorkerCommandPanel _workerCommandPanel;
    [SerializeField] private HousePanel _housePanel;
    [SerializeField] private ProductionBuildingPanel _productionBuildingPanel;
    [SerializeField] private ArmySelectionPanel _armySelectionPanel;
    [SerializeField] private WorkerListPanel _workerListPanel;
    [SerializeField] private SelectionUiPresenter _selectionUiPresenter;

    public override void InstallBindings()
    {
        BindCoreServices();
        BindRegistries();
        BindSelectionAndCamera();
        BindUi();
        BindGameplayServices();
        BindFactories();
        BindInputAndCommands();
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

    private void BindCoreServices()
    {
        Container.Bind<ResourceStorage>()
            .FromInstance(_resourceStorage)
            .AsSingle()
            .NonLazy();
    }

    private void BindRegistries()
    {
        Container.Bind<WorkerRegistry>()
            .FromInstance(_workerRegistry)
            .AsSingle()
            .NonLazy();

        Container.Bind<ResourceRegistry>()
            .FromInstance(_resourceRegistry)
            .AsSingle()
            .NonLazy();

        Container.Bind<BuildingRegistry>()
            .FromInstance(_buildingRegistry)
            .AsSingle()
            .NonLazy();

        Container.Bind<ArmyUnitRegistry>()
            .FromInstance(_armyUnitRegistry)
            .AsSingle()
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

        Container.Bind<CameraFocusController>()
            .FromInstance(_cameraFocusController)
            .AsSingle()
            .NonLazy();
    }

    private void BindUi()
    {
        Container.Bind<Canvas>()
            .FromInstance(_screenCanvas)
            .AsSingle()
            .NonLazy();

        Container.Bind<SelectionUiPresenter>()
            .FromInstance(_selectionUiPresenter)
                 .AsSingle()
                 .NonLazy();

        Container.Bind<ConstructionPanel>()
            .FromInstance(_constructionPanel)
            .AsSingle()
            .NonLazy();

        Container.Bind<WorkerCommandPanel>()
            .FromInstance(_workerCommandPanel)
            .AsSingle()
            .NonLazy();

        Container.Bind<HousePanel>()
            .FromInstance(_housePanel)
            .AsSingle()
            .NonLazy();

        Container.Bind<ProductionBuildingPanel>()
            .FromInstance(_productionBuildingPanel)
            .AsSingle()
            .NonLazy();

        Container.Bind<ArmySelectionPanel>()
            .FromInstance(_armySelectionPanel)
            .AsSingle()
            .NonLazy();

        Container.Bind<WorkerListPanel>()
            .FromInstance(_workerListPanel)
            .AsSingle()
            .NonLazy();
    }

    private void BindGameplayServices()
    {
        Container.Bind<WorkerJobFactory>()
            .AsSingle()
            .NonLazy();

        Container.Bind<WorkerCommandService>()
            .AsSingle()
            .NonLazy();
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
