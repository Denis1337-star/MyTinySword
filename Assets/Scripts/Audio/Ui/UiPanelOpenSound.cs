using UnityEngine;

/// <summary>
/// Проигрывает UI звук при открытии панели
/// </summary>
public sealed class UiPanelOpenSound : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private SoundId _soundId = SoundId.PanelOpen;

    [Tooltip("Если включено, звук не проиграется при самом первом OnEnable после загрузки сцены")]
    [SerializeField] private bool _skipFirstEnable = true;

    private bool _wasEnabledOnce;

    private void OnEnable()
    {
        if (_skipFirstEnable && !_wasEnabledOnce)
        {
            _wasEnabledOnce = true;
            return;
        }

        PlayOpenSound();
    }

    private void PlayOpenSound()
    {
        if (_soundId == SoundId.None)
            return;

        GameAudioService audioService = GameAudioService.Instance;

        if (audioService == null)
            return;

        audioService.PlayUiSound(_soundId);
    }
}