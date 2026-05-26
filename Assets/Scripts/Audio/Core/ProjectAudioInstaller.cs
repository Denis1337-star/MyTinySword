using UnityEngine;
using Zenject;

/// <summary>
/// Глобальный installer audio системы
/// </summary>
public sealed class ProjectAudioInstaller : MonoInstaller
{
    [SerializeField] private GameAudioService _audioServicePrefab;

    public override void InstallBindings()
    {
        BindAudioService();
    }

    private void BindAudioService()
    {
        Container
            .Bind<GameAudioService>()
            .FromComponentInNewPrefab(_audioServicePrefab)
            .AsSingle()
            .NonLazy();
    }
}