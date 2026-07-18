using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Подключает UI-клик ко всем Button под root.
/// Статические кнопки — через WireAllButtons в Awake.
/// Динамические — через WireButton после Instantiate.
/// </summary>
public sealed class UiSoundRouter : ValidatedMonoBehaviour
{
    [SerializeField] private Transform _buttonsRoot;
    [SerializeField] private SoundId _soundId = SoundId.ButtonClick;
    [SerializeField] private bool _playOnlyWhenInteractable = true;

    private readonly Dictionary<Button, UnityAction> _handlers = new();

    private GameAudioService _audioService;

    [Inject]
    private void Construct(GameAudioService audioService)
    {
        _audioService = audioService;
    }

    protected override void Awake()
    {
        base.Awake();
        EnsureAudioService();
        WireAllButtons();
    }

    private void OnDestroy()
    {
        UnwireAll();
    }

    protected override bool ValidateInternal()
    {
        return true;
    }

    public void WireAllButtons()
    {
        Transform root = _buttonsRoot != null ? _buttonsRoot : transform;
        Button[] buttons = root.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
            WireButton(buttons[i]);
    }

    public void WireButton(Button button)
    {
        if (button == null || _handlers.ContainsKey(button))
            return;

        UnityAction handler = () => PlayFor(button);
        _handlers.Add(button, handler);
        button.onClick.AddListener(handler);
    }

    public void UnwireButton(Button button)
    {
        if (button == null || !_handlers.TryGetValue(button, out UnityAction handler))
            return;

        button.onClick.RemoveListener(handler);
        _handlers.Remove(button);
    }

    private void UnwireAll()
    {
        foreach (KeyValuePair<Button, UnityAction> pair in _handlers)
        {
            if (pair.Key != null)
                pair.Key.onClick.RemoveListener(pair.Value);
        }

        _handlers.Clear();
    }

    private void PlayFor(Button button)
    {
        if (button == null)
            return;

        if (_playOnlyWhenInteractable && !button.interactable)
            return;

        if (_audioService == null)
            EnsureAudioService();

        if (_audioService == null)
            return;

        _audioService.PlayUiSound(_soundId);
    }

    private void EnsureAudioService()
    {
        if (_audioService != null)
            return;

        if (!ProjectContext.HasInstance)
            return;

        _audioService = ProjectContext.Instance.Container.TryResolve<GameAudioService>();
    }
}
