using System.Collections;
using UnityEngine;
using Zenject;

/// <summary>
/// Загружает аудио настройки после инициализации PluginYG2 и GameAudioService
/// </summary>
public sealed class AudioSaveBootstrap : MonoBehaviour
{
    private GameAudioService _audioService;
    private YandexAudioSaveService _audioSaveService;

    [Inject]
    private void Construct(
        GameAudioService audioService,
        YandexAudioSaveService audioSaveService)
    {
        _audioService = audioService;
        _audioSaveService = audioSaveService;
    }

    private IEnumerator Start()
    {
        yield return null;

        _audioSaveService.LoadTo(_audioService);
    }
}