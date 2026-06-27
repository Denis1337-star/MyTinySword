using UnityEngine;
using Zenject;

/// <summary>
/// Глобальный installer audio-системы.
/// </summary>
public sealed class ProjectServicesInstaller : MonoInstaller
{
    [SerializeField] private GameAudioService _audioServicePrefab;
    [SerializeField] private TechTreeCatalogConfig _techTreeCatalog;

    public override void InstallBindings()
    {
        if (!ValidateReferences())
            return;

        Container
         .Bind<GameAudioService>()
         .FromComponentInNewPrefab(_audioServicePrefab)
         .AsSingle()
         .NonLazy();

        Container
            .Bind<GamePauseService>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<IAdvertisementService>()
            .To<YandexAdvertisementService>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<YandexAudioSaveService>()
            .AsSingle()
            .NonLazy();

        Container
    .BindInterfacesTo<AudioSaveInitializer>()
    .AsSingle()
    .NonLazy();

        Container
              .Bind<LevelProgressService>()
              .AsSingle()
              .NonLazy();

        Container
             .Bind<LevelLoaderService>()
              .AsSingle()
              .NonLazy();

        Container
             .Bind<LevelRuntimeService>()
             .AsSingle()
             .NonLazy();

        Container
             .Bind<TutorialSaveService>()
             .AsSingle()
              .NonLazy();

        Container
            .Bind<TechTreeTimeService>()
            .AsSingle()
            .NonLazy();

        Container
           .Bind<TechTreeSaveService>()
           .AsSingle()
           .NonLazy();

        Container
      .Bind<TechTreeCatalogConfig>()
      .FromInstance(_techTreeCatalog)
      .AsSingle()
      .NonLazy();

        Container
     .Bind<TechTreeBonusService>()
     .AsSingle()
     .NonLazy();

    }
    private bool ValidateReferences()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _audioServicePrefab, nameof(_audioServicePrefab));
        valid &= ValidationUtility.IsValidConfig(this, _techTreeCatalog, nameof(_techTreeCatalog));

        return valid;
    }
}
