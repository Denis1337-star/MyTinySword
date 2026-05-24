using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Проигрывает UI звук при нажатии на кнопку
/// </summary>
[RequireComponent(typeof(Button))]
public sealed class UiButtonSound : ValidatedMonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private SoundId _soundId = SoundId.ButtonClick;
    [SerializeField] private bool _playOnlyWhenInteractable = true;

    protected override void Awake()
    {
        ResolveReferences();

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

    private void Reset()
    {
        ResolveReferences();
    }

    private void OnValidate()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (_button == null)
            _button = GetComponent<Button>();
    }

    private void PlaySound()
    {
        if (_playOnlyWhenInteractable && !_button.interactable)
            return;

        GameAudioService audioService = GameAudioService.Instance;

        if (audioService == null)
            return;

        audioService.PlayUiSound(_soundId);
    }
}