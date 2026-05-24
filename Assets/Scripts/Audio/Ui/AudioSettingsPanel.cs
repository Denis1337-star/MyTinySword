using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI панель настроек звука
/// </summary>
public sealed class AudioSettingsPanel : ValidatedMonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject _root;

    [Header("Sliders")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Mute Toggles")]
    [SerializeField] private Toggle _musicMuteToggle;
    [SerializeField] private Toggle _sfxMuteToggle;

    private GameAudioService _audioService;
    private bool _isRefreshingUi;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        _audioService = GameAudioService.Instance;

        SubscribeUi();

        if (_audioService != null)
            _audioService.SettingsChanged += RefreshFromService;

        RefreshFromService();
    }

    private void OnDisable()
    {
        UnsubscribeUi();

        if (_audioService != null)
            _audioService.SettingsChanged -= RefreshFromService;
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _root, nameof(_root));
        valid &= ValidationUtility.IsAssigned(this, _musicSlider, nameof(_musicSlider));
        valid &= ValidationUtility.IsAssigned(this, _sfxSlider, nameof(_sfxSlider));
        valid &= ValidationUtility.IsAssigned(this, _musicMuteToggle, nameof(_musicMuteToggle));
        valid &= ValidationUtility.IsAssigned(this, _sfxMuteToggle, nameof(_sfxMuteToggle));

        return valid;
    }

    public void Show()
    {
        ShowRoot();
        RefreshFromService();
    }

    public void Hide()
    {
        HideRoot();
    }

    public void Toggle()
    {
        if (_root.activeSelf)
        {
            Hide();
            return;
        }

        Show();
    }

    private void SubscribeUi()
    {
        _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);

        _musicMuteToggle.onValueChanged.AddListener(OnMusicMuteToggleChanged);
        _sfxMuteToggle.onValueChanged.AddListener(OnSfxMuteToggleChanged);
    }

    private void UnsubscribeUi()
    {
        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);

        _musicMuteToggle.onValueChanged.RemoveListener(OnMusicMuteToggleChanged);
        _sfxMuteToggle.onValueChanged.RemoveListener(OnSfxMuteToggleChanged);
    }

    private void RefreshFromService()
    {
        if (_audioService == null)
            return;

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

        if (_audioService == null)
            return;

        _audioService.SetMusicVolume(value);
    }

    private void OnSfxSliderChanged(float value)
    {
        if (_isRefreshingUi)
            return;

        if (_audioService == null)
            return;

        _audioService.SetSfxVolume(value);
    }

    private void OnMusicMuteToggleChanged(bool muted)
    {
        if (_isRefreshingUi)
            return;

        if (_audioService == null)
            return;

        _audioService.SetMusicMuted(muted);
    }

    private void OnSfxMuteToggleChanged(bool muted)
    {
        if (_isRefreshingUi)
            return;

        if (_audioService == null)
            return;

        _audioService.SetSfxMuted(muted);
    }

    private void ShowRoot()
    {
        _root.SetActive(true);
    }

    private void HideRoot()
    {
        _root.SetActive(false);
    }
}