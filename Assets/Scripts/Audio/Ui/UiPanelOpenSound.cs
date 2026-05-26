using UnityEngine;
using Zenject;

/// <summary>
/// ѕроигрывает UI звук при открытии панели
/// </summary>
public sealed class UiPanelOpenSound : MonoBehaviour
{
    [SerializeField] private SoundId _soundId = SoundId.PanelOpen;

    private GameAudioService _audioService;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    private void OnEnable()
    {
        PlayOpenSound();
    }

    private void PlayOpenSound()
    {
        if (_soundId == SoundId.None)
            return;

        _audioService.PlayUiSound(_soundId);
    }
}