using Zenject;

/// <summary>
/// Загружает audio-настройки из YG2 после инициализации DI
/// </summary>
public sealed class AudioSaveInitializer : IInitializable
{
    private readonly GameAudioService _audioService;
    private readonly YandexAudioSaveService _audioSaveService;

    public AudioSaveInitializer(
        GameAudioService audioService,
        YandexAudioSaveService audioSaveService)
    {
        _audioService = audioService;
        _audioSaveService = audioSaveService;
    }

    public void Initialize()
    {
        _audioSaveService.LoadTo(_audioService);
    }
}