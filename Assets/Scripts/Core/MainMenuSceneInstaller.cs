using UnityEngine;
using Zenject;

/// <summary>
/// Installer сцены MainMenu.
/// </summary>
public sealed class MainMenuSceneInstaller : MonoInstaller
{
    [SerializeField] private UiSoundRouter _uiSoundRouter;

    public override void InstallBindings()
    {
        if (_uiSoundRouter == null)
            _uiSoundRouter = FindFirstObjectByType<UiSoundRouter>();

        if (_uiSoundRouter == null)
        {
            Debug.LogError($"{name}: UiSoundRouter не назначен.", this);
            return;
        }

        Container.Bind<UiSoundRouter>()
            .FromInstance(_uiSoundRouter)
            .AsSingle()
            .NonLazy();
    }
}
