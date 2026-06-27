using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Главный сервис звука
/// </summary>
public sealed class GameAudioService : ValidatedMonoBehaviour
{
    private const float MinVolume = 0.0001f;
    private const float MutedDb = -80f;

    [SerializeField] private AudioConfig _config;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _uiSfxSource;
    [SerializeField] private Transform _worldSfxRoot;

    private readonly List<AudioSource> _worldSources = new();
    private readonly List<WorldSoundPlayback> _recentWorldSounds = new();

    [SerializeField, Min(0f)] private float _sameWorldSoundCooldown = 0.08f;
    [SerializeField, Min(0f)] private float _sameWorldSoundDistance = 0.75f;

    private AudioListener _audioListener;
    private int _nextWorldSourceIndex;
    private bool _audioUnlocked;
    private string _pendingMusicSceneName;

    private float _lastNonZeroMusicVolume = 1f;
    private float _lastNonZeroSfxVolume = 1f;

    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }

    public bool IsMusicMuted { get; private set; }
    public bool IsSfxMuted { get; private set; }

    public event Action SettingsChanged;

    private readonly struct WorldSoundPlayback
    {
        public readonly SoundId SoundId;
        public readonly Vector3 Position;
        public readonly float Time;

        public WorldSoundPlayback(SoundId soundId, Vector3 position, float time)
        {
            SoundId = soundId;
            Position = position;
            Time = time;
        }
    }
    protected override void Awake()
    {
        base.Awake();

        InitializeAudioUnlockState();
        InitializeSources();
        InitializeVolumes();
        BuildWorldSfxPool();
        RefreshAudioListener();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }
    private void Update()
    {
        TryUnlockAudioFromUserGesture();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        ClearWorldSfxPool();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _config, nameof(_config));
        valid &= ValidationUtility.IsAssigned(this, _musicSource, nameof(_musicSource));
        valid &= ValidationUtility.IsAssigned(this, _uiSfxSource, nameof(_uiSfxSource));
        valid &= ValidationUtility.IsAssigned(this, _worldSfxRoot, nameof(_worldSfxRoot));

        if (_config != null && !_config.IsValid())
            valid = false;

        return valid;
    }

    /// <summary>
    /// Меняет громкость музыки
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);

        if (MusicVolume > MinVolume)
        {
            IsMusicMuted = false;
            _lastNonZeroMusicVolume = MusicVolume;
        }
        else
        {
            MusicVolume = 0f;
            IsMusicMuted = true;
        }

        ApplyMusicVolume();
        SettingsChanged?.Invoke();
    }

    /// <summary>
    /// Меняет громкость SFX
    /// </summary>
    public void SetSfxVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);

        if (SfxVolume > MinVolume)
        {
            IsSfxMuted = false;
            _lastNonZeroSfxVolume = SfxVolume;
        }
        else
        {
            SfxVolume = 0f;
            IsSfxMuted = true;
        }

        ApplySfxVolume();
        SettingsChanged?.Invoke();
    }

    /// <summary>
    /// Включает или выключает mute музыки
    /// </summary>
    public void SetMusicMuted(bool muted)
    {
        if (muted)
        {
            if (MusicVolume > MinVolume)
                _lastNonZeroMusicVolume = MusicVolume;

            MusicVolume = 0f;
            IsMusicMuted = true;
        }
        else
        {
            IsMusicMuted = false;

            if (MusicVolume <= MinVolume)
                MusicVolume = Mathf.Clamp01(_lastNonZeroMusicVolume);
        }

        ApplyMusicVolume();
        SettingsChanged?.Invoke();
    }

    /// <summary>
    /// Включает или выключает mute SFX
    /// </summary>
    public void SetSfxMuted(bool muted)
    {
        if (muted)
        {
            if (SfxVolume > MinVolume)
                _lastNonZeroSfxVolume = SfxVolume;

            SfxVolume = 0f;
            IsSfxMuted = true;
        }
        else
        {
            IsSfxMuted = false;

            if (SfxVolume <= MinVolume)
                SfxVolume = Mathf.Clamp01(_lastNonZeroSfxVolume);
        }

        ApplySfxVolume();
        SettingsChanged?.Invoke();
    }

    /// <summary>
    /// Проигрывает UI звук
    /// </summary>
    public void PlayUiSound(SoundId id)
    {
        if (!CanPlaySfx())
            return;

        SoundEntry entry = _config.GetSound(id);

        if (entry == null)
            return;

        AudioClip clip = entry.GetClip();

        if (clip == null)
            return;

        _uiSfxSource.pitch = entry.GetPitch();
        _uiSfxSource.PlayOneShot(clip, entry.Volume);
    }

    /// <summary>
    /// Проигрывает world звук в указанной 2D позиции
    /// </summary>
    public void PlayWorldSound(SoundId id, Vector2 worldPosition)
    {
        PlayWorldSound(id, new Vector3(worldPosition.x, worldPosition.y, 0f));
    }

    /// <summary>
    /// Проигрывает world звук в указанной позиции
    /// </summary>
    public void PlayWorldSound(SoundId id, Vector3 worldPosition)
    {
        if(!CanPlaySfx())
        return;

        Vector3 soundPosition = worldPosition;
        soundPosition.z = GetListenerZ();

        if (!CanPlayWorldSoundNow(id, soundPosition))
            return;

        SoundEntry entry = _config.GetSound(id);

        if (entry == null)
            return;

        AudioClip clip = entry.GetClip();

        if (clip == null)
            return;

        AudioSource source = GetAvailableWorldSource();

        if (source == null)
            return;

        source.transform.position = soundPosition;
        source.volume = entry.Volume;
        source.pitch = entry.GetPitch();

        // PlayOneShot не требует Stop(), поэтому мы не обрываем звук резко.
        source.PlayOneShot(clip, entry.Volume);

        RegisterWorldSound(id, soundPosition);
    }

    /// <summary>
    /// Включает музыку для указанной сцены
    /// </summary>
    public void PlayMusicForScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        if (!CanUseAudioNow())
        {
            _pendingMusicSceneName = sceneName;
            return;
        }

        AudioClip clip = _config.GetMusicForScene(sceneName);

        if (clip == null)
        {
            StopMusic();
            return;
        }

        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    /// <summary>
    /// Останавливает текущую музыку
    /// </summary>
    public void StopMusic()
    {
        _musicSource.Stop();
        _musicSource.clip = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshAudioListener();
        PlayMusicForScene(scene.name);
    }

    private void InitializeSources()
    {
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f;
        _musicSource.outputAudioMixerGroup = _config.MusicMixerGroup;

        _uiSfxSource.playOnAwake = false;
        _uiSfxSource.loop = false;
        _uiSfxSource.spatialBlend = 0f;
        _uiSfxSource.outputAudioMixerGroup = _config.SfxMixerGroup;
    }

    private void InitializeVolumes()
    {
        MusicVolume = Mathf.Clamp01(_config.DefaultMusicVolume);
        SfxVolume = Mathf.Clamp01(_config.DefaultSfxVolume);

        _lastNonZeroMusicVolume = MusicVolume > MinVolume
            ? MusicVolume
            : 1f;

        _lastNonZeroSfxVolume = SfxVolume > MinVolume
            ? SfxVolume
            : 1f;

        IsMusicMuted = MusicVolume <= MinVolume;
        IsSfxMuted = SfxVolume <= MinVolume;

        ApplyMusicVolume();
        ApplySfxVolume();
    }

    private void BuildWorldSfxPool()
    {
        ClearWorldSfxPool();

        int poolSize = Mathf.Max(1, _config.WorldSfxPoolSize);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject sourceObject = new GameObject($"WorldSfxSource_{i + 1}");
            sourceObject.transform.SetParent(_worldSfxRoot, false);

            AudioSource source = sourceObject.AddComponent<AudioSource>();
            ConfigureWorldSource(source);

            _worldSources.Add(source);
        }
    }

    private void ClearWorldSfxPool()
    {
        for (int i = 0; i < _worldSources.Count; i++)
            Destroy(_worldSources[i].gameObject);

        _worldSources.Clear();
        _nextWorldSourceIndex = 0;
    }

    private void ConfigureWorldSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = false;

        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = _config.WorldMinDistance;
        source.maxDistance = _config.WorldMaxDistance;
        source.dopplerLevel = 0f;

        source.outputAudioMixerGroup = _config.SfxMixerGroup;
    }

    private AudioSource GetAvailableWorldSource()
    {
        if (_worldSources.Count == 0)
            return null;

        for (int i = 0; i < _worldSources.Count; i++)
        {
            int index = (_nextWorldSourceIndex + i) % _worldSources.Count;
            AudioSource source = _worldSources[index];

            if (source.isPlaying)
                continue;

            _nextWorldSourceIndex = index + 1;

            if (_nextWorldSourceIndex >= _worldSources.Count)
                _nextWorldSourceIndex = 0;

            return source;
        }

        // Если все источники заняты, лучше пропустить новый звук,
        // чем резко оборвать старый и получить щелчок/скрежет.
        return null;
    }
    private bool CanPlayWorldSoundNow(SoundId soundId, Vector3 position)
    {
        if (_sameWorldSoundCooldown <= 0f)
            return true;

        CleanupRecentWorldSounds();

        float minDistanceSqr = _sameWorldSoundDistance * _sameWorldSoundDistance;
        float currentTime = Time.unscaledTime;

        for (int i = 0; i < _recentWorldSounds.Count; i++)
        {
            WorldSoundPlayback playback = _recentWorldSounds[i];

            if (playback.SoundId != soundId)
                continue;

            if (currentTime - playback.Time > _sameWorldSoundCooldown)
                continue;

            float distanceSqr = (playback.Position - position).sqrMagnitude;

            if (distanceSqr <= minDistanceSqr)
                return false;
        }

        return true;
    }

    private void RegisterWorldSound(SoundId soundId, Vector3 position)
    {
        _recentWorldSounds.Add(new WorldSoundPlayback(
            soundId,
            position,
            Time.unscaledTime));
    }

    private void CleanupRecentWorldSounds()
    {
        if (_recentWorldSounds.Count == 0)
            return;

        float currentTime = Time.unscaledTime;
        float maxLifetime = Mathf.Max(_sameWorldSoundCooldown, 0.1f);

        for (int i = _recentWorldSounds.Count - 1; i >= 0; i--)
        {
            if (currentTime - _recentWorldSounds[i].Time > maxLifetime)
                _recentWorldSounds.RemoveAt(i);
        }
    }

    private void ApplyMusicVolume()
    {
        SetMixerVolume(_config.MusicVolumeParameter, MusicVolume, IsMusicMuted);
    }

    private void ApplySfxVolume()
    {
        SetMixerVolume(_config.SfxVolumeParameter, SfxVolume, IsSfxMuted);
    }

    private void SetMixerVolume(string parameterName, float volume, bool muted)
    {
        float db = muted || volume <= MinVolume
            ? MutedDb
            : Mathf.Log10(volume) * 20f;

        _config.AudioMixer.SetFloat(parameterName, db);
    }

    private bool CanPlaySfx()
    {
        return CanUseAudioNow() && !IsSfxMuted && SfxVolume > MinVolume;
    }

    private void RefreshAudioListener()
    {
        _audioListener = FindObjectOfType<AudioListener>();
    }

    private float GetListenerZ()
    {
        if (_audioListener != null)
            return _audioListener.transform.position.z;

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
            return mainCamera.transform.position.z;

        return transform.position.z;
    }
    private void InitializeAudioUnlockState()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    _audioUnlocked = false;
#else
        _audioUnlocked = true;
#endif

        _pendingMusicSceneName = null;
    }

    private bool CanUseAudioNow()
    {
        return _audioUnlocked;
    }

    private void TryUnlockAudioFromUserGesture()
    {
        if (_audioUnlocked)
            return;

        if (!HasUserGestureThisFrame())
            return;

        _audioUnlocked = true;

        if (string.IsNullOrWhiteSpace(_pendingMusicSceneName))
            return;

        string sceneName = _pendingMusicSceneName;
        _pendingMusicSceneName = null;

        PlayMusicForScene(sceneName);
    }

    private bool HasUserGestureThisFrame()
    {
        Mouse mouse = Mouse.current;

        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            return true;

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.anyKey.wasPressedThisFrame)
            return true;

        Touchscreen touchscreen = Touchscreen.current;

        if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
            return true;

        Pen pen = Pen.current;

        if (pen != null && pen.tip.wasPressedThisFrame)
            return true;

        return false;
    }
}