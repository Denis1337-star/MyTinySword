using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI панель настроек звука
/// </summary>
public sealed class AudioSettingsPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;
    [SerializeField] private SimplePanelTween _panelTween;

    [Header("Music")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Toggle _musicMuteToggle;

    [Header("SFX")]
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _sfxMuteToggle;

    [Header("Save")]
    [SerializeField, Min(0.1f)] private float _saveDelay = 0.5f;

    private GameAudioService _audioService;
    private YandexAudioSaveService _audioSaveService;

    private Coroutine _saveRoutine;

    private bool _isRefreshingUi;
    private bool _uiSubscribed;
    private bool _audioServiceSubscribed;

    [Inject]
    private void Construct(
        GameAudioService audioService,
        YandexAudioSaveService audioSaveService)
    {
        _audioService = audioService;
        _audioSaveService = audioSaveService;

        TrySubscribeToAudioService();
        RefreshFromService();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        SubscribeUi();
        TrySubscribeToAudioService();
        RefreshFromService();
    }

    private void OnDisable()
    {
        UnsubscribeUi();
        UnsubscribeFromAudioService();
        StopSaveRoutine();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _musicSlider, nameof(_musicSlider));
        valid &= ValidationUtility.IsAssigned(this, _sfxSlider, nameof(_sfxSlider));
        valid &= ValidationUtility.IsAssigned(this, _musicMuteToggle, nameof(_musicMuteToggle));
        valid &= ValidationUtility.IsAssigned(this, _sfxMuteToggle, nameof(_sfxMuteToggle));
        valid &= ValidationUtility.IsAssigned(this, _panelTween, nameof(_panelTween));

        return valid;
    }

    public void Show()
    {
        _panelTween.Show();
        RefreshFromService();
    }

    public void Hide()
    {
        _panelTween.Hide();
    }

    public void Toggle()
    {
        if (_panelTween.IsVisible)
        {
            Hide();
            return;
        }

        Show();
    }

    private void SubscribeUi()
    {
        if (_uiSubscribed)
            return;

        _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);

        _musicMuteToggle.onValueChanged.AddListener(OnMusicMuteToggleChanged);
        _sfxMuteToggle.onValueChanged.AddListener(OnSfxMuteToggleChanged);

        _uiSubscribed = true;
    }

    private void UnsubscribeUi()
    {
        if (!_uiSubscribed)
            return;

        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);

        _musicMuteToggle.onValueChanged.RemoveListener(OnMusicMuteToggleChanged);
        _sfxMuteToggle.onValueChanged.RemoveListener(OnSfxMuteToggleChanged);

        _uiSubscribed = false;
    }

    private void TrySubscribeToAudioService()
    {
        if (_audioServiceSubscribed)
            return;

        _audioService.SettingsChanged += RefreshFromService;
        _audioServiceSubscribed = true;
    }

    private void UnsubscribeFromAudioService()
    {
        if (!_audioServiceSubscribed)
            return;

        if (_audioService != null)
            _audioService.SettingsChanged -= RefreshFromService;

        _audioServiceSubscribed = false;
    }

    private void RefreshFromService()
    {
        _isRefreshingUi = true;

        _musicSlider.SetValueWithoutNotify(_audioService.MusicVolume);
        _sfxSlider.SetValueWithoutNotify(_audioService.SfxVolume);

        _musicMuteToggle.SetIsOnWithoutNotify(_audioService.IsMusicMuted);
        _sfxMuteToggle.SetIsOnWithoutNotify(_audioService.IsSfxMuted);

        _isRefreshingUi = false;
    }

    private void OnMusicSliderChanged(float value)
    {
        if (_isRefreshingUi)
            return;

        _audioService.SetMusicVolume(value);
        ScheduleSave();
    }

    private void OnSfxSliderChanged(float value)
    {
        if (_isRefreshingUi)
            return;

        _audioService.SetSfxVolume(value);
        ScheduleSave();
    }

    private void OnMusicMuteToggleChanged(bool muted)
    {
        if (_isRefreshingUi)
            return;

        _audioService.SetMusicMuted(muted);
        ScheduleSave();
    }

    private void OnSfxMuteToggleChanged(bool muted)
    {
        if (_isRefreshingUi)
            return;

        _audioService.SetSfxMuted(muted);
        ScheduleSave();
    }

    private void ScheduleSave()
    {
        StopSaveRoutine();
        _saveRoutine = StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_saveDelay);

        _audioSaveService.SaveFrom(_audioService);
        _saveRoutine = null;
    }

    private void StopSaveRoutine()
    {
        if (_saveRoutine == null)
            return;

        StopCoroutine(_saveRoutine);
        _saveRoutine = null;
    }
}