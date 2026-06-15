using UnityEngine;
using Zenject;

/// <summary>
/// Глобальный installer audio системы
/// </summary>
public sealed class ProjectServicesInstaller : MonoInstaller
{
    [SerializeField] private GameAudioService _audioServicePrefab;

    public override void InstallBindings()
    {
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
    }
}
