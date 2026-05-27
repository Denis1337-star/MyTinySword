using UnityEngine;
using Zenject;

/// <summary>
/// ѕроигрывает UI звук при открытии панели
/// </summary>
public sealed class UiPanelOpenSound : MonoBehaviour
{
    [SerializeField] private SoundId _soundId = SoundId.PanelOpen;

    private GameAudioService _audioService;
    private bool _isConstructed;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
        _isConstructed = true;
    }

    private void OnEnable()
    {
        if (!_isConstructed)
            return;

        PlayOpenSound();
    }

    private void PlayOpenSound()
    {
        if (_soundId == SoundId.None)
            return;

        _audioService.PlayUiSound(_soundId);
    }
}