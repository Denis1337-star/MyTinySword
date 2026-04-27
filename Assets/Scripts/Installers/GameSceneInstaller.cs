using UnityEngine;
using Zenject;

/// <summary>
/// Главный installer сцены
/// </summary>
public class GameSceneInstaller : MonoInstaller
{
    [Header("Scene Services")]
    [SerializeField] private ResourceStorage _resourceStorage;
    [SerializeField] private ResourceDepositService _resourceDepositService;
    [SerializeField] private ResourceRegistry _resourceRegistry;

    [SerializeField] private ArmyUnitRegistry _armyUnitRegistry;
    [SerializeField] private WorkerRegistry _workerRegistry;

    [SerializeField] private BuildingRegistry _buildingRegistry;

    [SerializeField] private SelectionSystem _selectionSystem;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraFocusController _cameraFocusController;

    [SerializeField] private Canvas _screenCanvas;
    [SerializeField] private ConstructionPanel _constructionPanel;
    [SerializeField] private WorkerCommandPanel _workerCommandPanel;
    [SerializeField] private HousePanel _housePanel;
    [SerializeField] private ProductionBuildingPanel _productionBuildingPanel;
    [SerializeField] private ArmySelectionPanel _armySelectionPanel;


    public override void InstallBindings()
    {
        BindResourceStorage();
        BindResourceDepositService();
        BindArmyUnitRegistry();
        BindWorkerRegistry();
        BindResourceRegistry();
        BindBuildingRegistry();

        BindResourceSearchService();

        BindWorkerFactory();
        BindWorkerJobFactory();
        BindBuildingFactory();
        BindArmyUnitFactory();

        BindSelectionSystem();
        BindMainCamera();
        BindCameraFocusController();

        BindScreenCanvas();
        BindConstructionPanel();
        BindWorkerCommandPanel();
        BindHousePanel();
        BindProductionBuildingPanel();
        BindArmySelectionPanel();
        BindWorkerCommandService();

    }

    private void BindArmyUnitRegistry()
    {
        Container.Bind<ArmyUnitRegistry>()
            .FromInstance(_armyUnitRegistry)
            .AsSingle()
            .NonLazy();
    }

    private void BindWorkerRegistry()
    {
        Container.Bind<WorkerRegistry>()
            .FromInstance(_workerRegistry)
            .AsSingle()
            .NonLazy();
    }

    private void BindWorkerFactory()
    {
        Container.Bind<WorkerFactory>()
            .AsSingle()
            .NonLazy();
    }
    private void BindWorkerJobFactory()
    {
        Container.Bind<WorkerJobFactory>()
            .AsSingle()
            .NonLazy();
    }
    private void BindResourceRegistry()
    {
        Container.Bind<ResourceRegistry>()
            .FromInstance(_resourceRegistry)
            .AsSingle()
            .NonLazy();
    }
    private void BindResourceSearchService()
    {
        Container.Bind<ResourceSearchService>()
            .AsSingle()
            .NonLazy();
    }
    private void BindResourceStorage()
    {
        Container.Bind<ResourceStorage>()
            .FromInstance(_resourceStorage)
            .AsSingle()
            .NonLazy();
    }

    private void BindResourceDepositService()
    {
        Container.Bind<ResourceDepositService>()
            .FromInstance(_resourceDepositService)
            .AsSingle()
            .NonLazy();
    }
    private void BindBuildingRegistry()
    {
        Container.Bind<BuildingRegistry>()
            .FromInstance(_buildingRegistry)
            .AsSingle()
            .NonLazy();
    }
    private void BindBuildingFactory()
    {
        Container.Bind<BuildingFactory>()
            .AsSingle()
            .NonLazy();
    }
    private void BindSelectionSystem()
    {
        Container.Bind<SelectionSystem>()
            .FromInstance(_selectionSystem)
            .AsSingle()
            .NonLazy();
    }
    private void BindMainCamera()
    {
        Container.Bind<Camera>()
            .FromInstance(_mainCamera)
            .AsSingle()
            .NonLazy();
    }
    private void BindCameraFocusController()
    {
        Container.Bind<CameraFocusController>()
            .FromInstance(_cameraFocusController)
            .AsSingle()
            .NonLazy();
    }
    private void BindScreenCanvas()
    {
        Container.Bind<Canvas>()
            .FromInstance(_screenCanvas)
            .AsSingle()
            .NonLazy();
    }
    private void BindConstructionPanel()
    {
        Container.Bind<ConstructionPanel>()
            .FromInstance(_constructionPanel)
            .AsSingle()
            .NonLazy();
    }
    private void BindWorkerCommandPanel()
    {
        Container.Bind<WorkerCommandPanel>()
            .FromInstance(_workerCommandPanel)
            .AsSingle()
            .NonLazy();
    }

    private void BindHousePanel()
    {
        Container.Bind<HousePanel>()
            .FromInstance(_housePanel)
            .AsSingle()
            .NonLazy();
    }

    private void BindProductionBuildingPanel()
    {
        Container.Bind<ProductionBuildingPanel>()
            .FromInstance(_productionBuildingPanel)
            .AsSingle()
            .NonLazy();
    }

    private void BindArmySelectionPanel()
    {
        Container.Bind<ArmySelectionPanel>()
            .FromInstance(_armySelectionPanel)
            .AsSingle()
            .NonLazy();
    }
    private void BindWorkerCommandService()
    {
        Container.Bind<WorkerCommandService>()
            .AsSingle()
            .NonLazy();
    }
    private void BindArmyUnitFactory()
    {
        Container.Bind<ArmyUnitFactory>()
            .AsSingle()
            .NonLazy();
    }
}
