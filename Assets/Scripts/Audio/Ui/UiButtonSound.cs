using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Проигрывает UI звук при нажатии на кнопку
/// </summary>
[RequireComponent(typeof(Button))]
public sealed class UiButtonSound : ValidatedMonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private SoundId _soundId = SoundId.ButtonClick;

    [Tooltip("Если включено, звук не будет проигрываться у неактивной кнопки")]
    [SerializeField] private bool _playOnlyWhenInteractable = true;

    private GameAudioService _audioService;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(PlaySound);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(PlaySound);
    }

    protected override bool ValidateInternal()
    {
        return ValidationUtility.IsAssigned(this, _button, nameof(_button));
    }

    private void PlaySound()
    {
        if (_playOnlyWhenInteractable && !_button.interactable)
            return;

        _audioService.PlayUiSound(_soundId);
    }
}